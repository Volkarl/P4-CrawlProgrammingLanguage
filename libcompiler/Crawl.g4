grammar Crawl;

tokens { INDENT, DEDENT }

//Code to handle INDENT/DEDENT instead of curly braces. Complained when i tried to move it so now it is at the top of file
//Acctual CFG starts on line 132

@lexer::header { 
using System.Collections.Generic; 
using System.Linq; 
using libcompiler.ExtensionMethods;
}
@lexer::members {
// A queue where extra tokens are pushed on (see the NEWLINE lexer rule).
  private List<IToken> tokens = new List<IToken>();
  // The stack that keeps track of the indentation level.
  private Stack<int> indents = new Stack<int>();
  // The amount of opened braces, brackets and parenthesis.
  private int opened = 0;
  // The most recently produced token.
  private IToken lastToken = null;
  
  private string Escape(string s)
  {
    StringBuilder sb = new StringBuilder();
    foreach (char c in s)
    {
      if (c == '\n')
        sb.Append("\\n");
      else if (c == '\r')
        sb.Append("\\r");
      else if (c == '\t')
        sb.Append("\\t");
      else sb.Append(c);
    }
    return sb.ToString();
  }
  
  public override void Emit(IToken t) {
	TraceListners.ParserTraceListner?.WriteLine(
        $"Emitting :{CrawlParser.DefaultVocabulary.GetSymbolicName(t.Type)}{(string.IsNullOrEmpty(t.Text) ? string.Empty : " \"" + Escape(t.Text) + "\"")}({t.Channel})", 
        "Lexer"
    );
	Token = t;
    tokens.Add(t);
  }

  public override IToken NextToken() {
    // Check if the end-of-file is ahead and there are still some DEDENTS expected.
    if (InputStream.LA(1) == IntStreamConstants.EOF && this.indents.Count > 0) {
      // Remove any trailing EOF tokens from our buffer.
      for (int i = tokens.Count - 1; i >= 0; i--) {
        if (tokens[i].Type == IntStreamConstants.EOF) {
          tokens.RemoveAt(i);
        }
      }

      // First emit an extra line break that serves as the end of the statement.
      this.Emit(CommonToken(CrawlParser.NEWLINE, "\n"));

      // Now emit as much DEDENT tokens as needed.
      while (indents.Count > 0) {
        this.Emit(CreateDedent());
        indents.Pop();
      }

      // Put the EOF back on the token stream.
      this.Emit(CommonToken(IntStreamConstants.EOF, "<EOF>"));
    }

    IToken next = base.NextToken();

    if (next.Channel == TokenConstants.DefaultChannel) {
      // Keep track of the last token on the default channel.
      this.lastToken = next;
    }

    return tokens.Count == 0 ? next : tokens.RemoveHead();
  }

  private IToken CreateDedent() {
    CommonToken dedent = CommonToken(CrawlParser.DEDENT, "DEDENT");
    dedent.Line = this.lastToken.Line ;
    return dedent;
  }

  private CommonToken CommonToken(int type, string text, int channel = TokenConstants.DefaultChannel) {
    int stop = this.CharIndex - 1;
    int start = text.Length == 0 ? stop : stop - text.Length + 1;
    return new CommonToken(
		new Antlr4.Runtime.Sharpen.Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream),
	    type, 
		channel, 
		start, 
		stop
	);
  }

  // Calculates the indentation of the provided spaces, taking the
  // following rules into account:
  //
  // "Tabs are replaced (from left to right) by one to eight spaces
  //  such that the total number of characters up to and including
  //  the replacement is a multiple of eight [...]"
  //
  //  -- https://docs.python.org/3.1/reference/lexical_analysis.html#indentation
  
  //TODO: Fix it. This is code from python, that belives tab = 8 spaces
  static int GetIndentationCount(string spaces) {
    int count = 0;
    foreach (char ch in spaces) {
      switch (ch) {
        case '\t':
          count += 8 - (count % 8);
          break;
        default:
          // A normal space char.
          count++;
		  break;
      }
    }

    return count;
  }

  bool AtStartOfInput() {
    return base.CharIndex == 0 && base.Line == 1;
	//TODO: MAKE SURE CHAR INDEX IS WHAT I THINK IT IS (char column)
  }
}

//The acctual CFG. 
//A translation unit is one source file for a program. First it contains imports of libraries, then the statements that make up the program
translation_unit		: import_directives namespace_declaration statements;

//////////////////////////////////////////////////////////////////////////////////
//import_directive(s) is imports. Using from C# or import from python
import_directives		: import_directive* ;
import_directive		: IMPORT IDENTIFIER (DOT IDENTIFIER)* END_OF_STATEMENT;

//////////////////////////////////////////////////////////////////////////////////
//Namespaces are made as packages. 
namespace_declaration	: (PACKAGE IDENTIFIER (DOT IDENTIFIER)* END_OF_STATEMENT)?;

//////////////////////////////////////////////////////////////////////////////////
//Statements make up the program. Functions/Classes, function calls and general computation
statements				: ( if_selection | for_loop | while_loop | declaration | assignment | return_statement | side_effect_stmt | END_OF_STATEMENT | NEWLINE ) *;

//////////////////////////////////////////////////////////////////////////////////
//A side effect statement is a statement with a side effect. Aka a method call. 
//A later part of the compiler needs to ensure it acctually ends with a call_expression
//Could _maybe_ be done in the parser, but it requires a lot of lookahead.
side_effect_stmt		: postfix_expression call_expression END_OF_STATEMENT;

//////////////////////////////////////////////////////////////////////////////////
//Next group of statements are the flow control statements. Loops and if's
//An if statement. Possibly with an else tacked on.
if_selection			: IF expression INDENT statements DEDENT (ELSE ((INDENT statements DEDENT) | if_selection))?;

//A for loop is in reality a foreach loop. Loops over a collection or range. Old school for loop is dead
for_loop				: FOR type IDENTIFIER FOR_LOOP_SEPERATOR expression INDENT statements DEDENT;

//Plain and simple while loop. While expression is true, whatever
while_loop				: WHILE expression INDENT statements DEDENT;

//Returns from a method. Optionally return a value
return_statement		: RETURN expression? END_OF_STATEMENT;
		
///////////////////////////////////////////////////////////////////////////////
//Since we try and treat methods as any other type, we can't quite see if it is a method or variable definiton before we read it.
//But this section deals with declearation of anything you can access at a later time
declaration				: protection_level? (class_declaration | method_decleration | variable_declerations|constructor_declaration) ;


method_decleration	: type parameters generic_parameters? IDENTIFIER ASSIGNMENT_SYMBOL method_body;
parameters              : LPARENTHESIS (parameter ( ITEM_SEPARATOR parameter )* )?  RPARENTHESIS;
parameter               : REFERENCE? type IDENTIFIER;
generic_parameters      : LANGLEBRACKET generic ( ITEM_SEPARATOR generic )* RANGLEBRACKET;
generic                 : IDENTIFIER ( INHERITANCE_OPERATOR IDENTIFIER )?;

variable_declerations	: type variable_decl (ITEM_SEPARATOR variable_decl)* END_OF_STATEMENT;
variable_decl			: IDENTIFIER (ASSIGNMENT_SYMBOL expression)? ;

//The body of a method. No great secrets hidden here
method_body			: INDENT statements DEDENT;

//Declearation of a class. A class starts with 'class' (well, translated) then its name, 
//then plausibly a list of things to inherit f rom. 
class_declaration		: CLASS IDENTIFIER inheritance generic_parameters? ASSIGNMENT_SYMBOL class_body;
//inheritances			: ( INHERITANCE_OPERATOR inheritance (ITEM_SEPARATOR inheritance)* )? ;
inheritance				: ( INHERITANCE_OPERATOR IDENTIFIER )?;
//The class body only allows decleartions, not the broader statements, we don't want to define wth happens with general computation in a class body
class_body				: INDENT declaration* DEDENT;


//////////////////////////////////////////////////////////////////////////////////
constructor_declaration	: parameters CONSTRUCT ASSIGNMENT_SYMBOL method_body; 

/////////////////////////////////////////////////////
//////////////////////////
//A few nuts and bolts that is also needed.

//Save some value in a variable
assignment				: (postfix_expression (subfield_expression | index_expression) | atom) ASSIGNMENT_SYMBOL expression END_OF_STATEMENT;

//A type. As a method is a type with "return_type (argument types)" the real decleartion of type is "type (list of types)?" but that is left recursive type : 
//Antlr can maybe acctually deal with this, but we just rewrite it
//Its a * and not a ? as a method can return a method, ad infinitum....

type					: IDENTIFIER method_type? array_type? generic_unpack_expression?;


//The tailing part if you define a method. ( optional reference, argument type, optional name, repeat)
//type_tail			:  | array_type ;
array_type			: (LSQUAREBRACKET ITEM_SEPARATOR* RSQUAREBRACKET)+ ;
method_type			: (LPARENTHESIS method_arguments?  RPARENTHESIS)+ ;
method_arguments	: (REFERENCE? type ( ITEM_SEPARATOR REFERENCE? type) *) ;

//Protection level. Just stolen from .NET, as we target CLR
protection_level		: PUBLIC | PRIVATE | PROTECTED | INTERNAL | PROTECTED_INTERNAL ;

//The expression circus. The reason there is a lot is to make sure it parses in the correct order.
//As an example, in or_expression, it is an and_expression (OR... )*. 
//This ensures that anything between OR is parsed independently (higher priorty, more grouped)
//I don't think i can explain it better, you really need the revelation yourself.

//A list of expressions (method calls ect)
ref_expression_list		: REFERENCE? expression (ITEM_SEPARATOR REFERENCE? expression)* ;

expression_list			: expression ( ITEM_SEPARATOR expression )* ;

expression				: range_expression;

range_expression		: or_expression (TO or_expression )?;

or_expression			: and_expression ( OR and_expression )* ;

and_expression			: comparison_expression ( AND comparison_expression )* ;

comparison_expression	: additive_expression (comparison_symbol additive_expression)? ;  //?

additive_expression		: multiplicative_expression ((PLUS | MINUS) multiplicative_expression )* ;

multiplicative_expression: exponential_expression (MULTIPLICATIVE_SYMBOL exponential_expression )* ;

exponential_expression	: cast_expression (EXPONENT cast_expression)* ;

cast_expression			: ( LPARENTHESIS type RPARENTHESIS ) * unary_expression ;

unary_expression		: ( INVERT | MINUS )* postfix_expression ;

postfix_expression		: atom ( call_expression | subfield_expression | index_expression | generic_unpack_expression)* ;

call_expression			: LPARENTHESIS ref_expression_list? RPARENTHESIS ;

subfield_expression		: DOT IDENTIFIER ;

index_expression		: LSQUAREBRACKET	expression_list RSQUAREBRACKET ;

generic_unpack_expression   : LANGLEBRACKET type ( ITEM_SEPARATOR type )* RANGLEBRACKET;

//An atom is an atom, a part that cannot be broken in smaller parts.
atom					: IDENTIFIER
						| literal
						| LPARENTHESIS expression RPARENTHESIS ;

///////////////////////////////////////////////////////////////////////////////

//More nuts and bolts
//Symbols used for different things. Should maybe be changed to tokens, but Antlr does magic and I don't.
comparison_symbol		: RANGLEBRACKET | '>=' | '==' | '!=' | '<=' | LANGLEBRACKET ;
MULTIPLICATIVE_SYMBOL	: '*' | '/' | '%' ;
MINUS					: '-' ;
PLUS					: '+' ;

//All the literals. Values
literal					: boolean_literal 
						| integer_literal
						| real_literal
						| string_literal;

//Boolean is true or false.
boolean_literal			: TRUE | FALSE ;
//An integer is a number...
integer_literal			: NUMBER;
//A real number is either a number containing a DOT or a number in scientific notion
real_literal			: POINT_REAL | EXPONENT_REAL;
//And finally, a string is a string...
string_literal			: STRING_LITERAL ;


POINT_REAL				: NUMBER? DOT NUMBER;
EXPONENT_REAL			: ( NUMBER | POINT_REAL ) EXPONENT_END ;
STRING_LITERAL			: '"' ( STRING_ESCAPE_SEQ | ~[\\\r\n"] )* '"' ;
TRUE					: 'sandt' ;
FALSE					: 'falsk' ;

///////////////////////////////////////////////////////////////////////////////
//Protection levels
PUBLIC					: 'offentlig' | 'offentligt' ;
PRIVATE					: 'privat' ;
PROTECTED				: 'beskyttet' ;
PROTECTED_INTERNAL		: 'beskyttet intern' ;
INTERNAL				: 'intern' ;

//keywords. refer to above to find out how they are used
CLASS					: 'klasse';
RETURN					: 'returner';
IF						: 'hvis';
ELSE					: 'ellers';
WHILE					: 'mens';
FOR						: 'for';
TO						: 'til';
AND						: 'og' ;
OR						: 'eller' ;
IMPORT					: 'importer' ;
REFERENCE				: 'reference' ;
CONSTRUCT				: 'opret' ;
PACKAGE					: 'pakke' ;

//Symbols with meaning
FOR_LOOP_SEPERATOR		: 'fra' ;
ITEM_SEPARATOR			: ',' ;
ASSIGNMENT_SYMBOL		: '=' ;
END_OF_STATEMENT		: ';' ;
LPARENTHESIS			: '(' {opened++;} ;
RPARENTHESIS			: ')'  {opened--;};
LSQUAREBRACKET			: '['  {opened++;};
RSQUAREBRACKET			: ']'  {opened--;};
LANGLEBRACKET           : '<' ;
RANGLEBRACKET           : '>' ;
INVERT					: 'ikke' ;
DOT						: '.' ;
EXPONENT				: '**' ;
INHERITANCE_OPERATOR	: 'er';

///////////////////////////////////////////////////////////////////////////////
//Finally some tokens that is more than just a specific string.

//A comment starts with // and then anything not a line change. It is sent to the hidden channel, ignored by the parser
COMMENT					: '//' ~( '\r' | '\n' )* -> channel(HIDDEN);
//Whitespace is also ignored by the parser
WS						: ( SPACES )+ -> channel(HIDDEN);

//A number...
NUMBER					: DIGIT+ ;

//An identifier. It can be used either as a type or a variable
IDENTIFIER 				: STARTSYMBOL SYMBOL* ;

//A fragment is a token that cannot be parsed, but can be used in other tokens
fragment SPACES			: [ \t]+ ;

fragment DIGIT 			: '0' .. '9' ;

fragment STRING_ESCAPE_SEQ : '\\' . ;

fragment EXPONENT_END	: ('e' |'E' ) (PLUS | MINUS)? NUMBER ;


//Code used to handle emitting DEDENT/INDENT after newlines. Newlines itself is hidden (ignored by the parser unless told not to)
NEWLINE
 : ( {AtStartOfInput()}?   SPACES
   | ( '\r'? '\n' | '\r' ) SPACES?
   )
   {
     string newLine = new string(Text.Where(x => x == '\n' || x == '\r').ToArray());
	 string spaces = new string(Text.Where(x => !(x == '\n' || x == '\r')).ToArray());
     int next = InputStream.LA(1);
     if (opened > 0 || next == '\r' || next == '\n' || next == '#') {
       // If we're inside a list or on a blank line, ignore all indents, 
       // dedents and line breaks.
       Skip();
     }
     else {
       Emit(CommonToken(NEWLINE, newLine, TokenConstants.HiddenChannel));
       int indent = GetIndentationCount(spaces);
       int previous = indents.Count == 0 ? 0 : indents.Peek();
       if (indent == previous) {
         // skip indents of the same size as the present indent-size
         Skip();
       }
       else if (indent > previous) {
         indents.Push(indent);
         Emit(CommonToken(CrawlParser.INDENT, spaces));
       }
       else {
         // Possibly emit more than 1 DEDENT token.
         while(indents.Count > 0 && indents.Peek() > indent) {
           this.Emit(CreateDedent());
           indents.Pop();
         }
       }
     }
   } -> channel(HIDDEN)
 ;
 

//Taken from https://github.com/antlr/grammars-v4/blob/master/python3/Python3.g4
//A lot of symbols that python thinks are reasonable unicode symbols in identifiers

fragment STARTSYMBOL 
 : '_'
 | [A-Z]
 | [a-z]
 | '\u00AA'
 | '\u00B5'
 | '\u00BA'
 | [\u00C0-\u00D6]
 | [\u00D8-\u00F6]
 | [\u00F8-\u01BA]
 | '\u01BB'
 | [\u01BC-\u01BF]
 | [\u01C0-\u01C3]
 | [\u01C4-\u0241]
 | [\u0250-\u02AF]
 | [\u02B0-\u02C1]
 | [\u02C6-\u02D1]
 | [\u02E0-\u02E4]
 | '\u02EE'
 | '\u037A'
 | '\u0386'
 | [\u0388-\u038A]
 | '\u038C'
 | [\u038E-\u03A1]
 | [\u03A3-\u03CE]
 | [\u03D0-\u03F5]
 | [\u03F7-\u0481]
 | [\u048A-\u04CE]
 | [\u04D0-\u04F9]
 | [\u0500-\u050F]
 | [\u0531-\u0556]
 | '\u0559'
 | [\u0561-\u0587]
 | [\u05D0-\u05EA]
 | [\u05F0-\u05F2]
 | [\u0621-\u063A]
 | '\u0640'
 | [\u0641-\u064A]
 | [\u066E-\u066F]
 | [\u0671-\u06D3]
 | '\u06D5'
 | [\u06E5-\u06E6]
 | [\u06EE-\u06EF]
 | [\u06FA-\u06FC]
 | '\u06FF'
 | '\u0710'
 | [\u0712-\u072F]
 | [\u074D-\u076D]
 | [\u0780-\u07A5]
 | '\u07B1'
 | [\u0904-\u0939]
 | '\u093D'
 | '\u0950'
 | [\u0958-\u0961]
 | '\u097D'
 | [\u0985-\u098C]
 | [\u098F-\u0990]
 | [\u0993-\u09A8]
 | [\u09AA-\u09B0]
 | '\u09B2'
 | [\u09B6-\u09B9]
 | '\u09BD'
 | '\u09CE'
 | [\u09DC-\u09DD]
 | [\u09DF-\u09E1]
 | [\u09F0-\u09F1]
 | [\u0A05-\u0A0A]
 | [\u0A0F-\u0A10]
 | [\u0A13-\u0A28]
 | [\u0A2A-\u0A30]
 | [\u0A32-\u0A33]
 | [\u0A35-\u0A36]
 | [\u0A38-\u0A39]
 | [\u0A59-\u0A5C]
 | '\u0A5E'
 | [\u0A72-\u0A74]
 | [\u0A85-\u0A8D]
 | [\u0A8F-\u0A91]
 | [\u0A93-\u0AA8]
 | [\u0AAA-\u0AB0]
 | [\u0AB2-\u0AB3]
 | [\u0AB5-\u0AB9]
 | '\u0ABD'
 | '\u0AD0'
 | [\u0AE0-\u0AE1]
 | [\u0B05-\u0B0C]
 | [\u0B0F-\u0B10]
 | [\u0B13-\u0B28]
 | [\u0B2A-\u0B30]
 | [\u0B32-\u0B33]
 | [\u0B35-\u0B39]
 | '\u0B3D'
 | [\u0B5C-\u0B5D]
 | [\u0B5F-\u0B61]
 | '\u0B71'
 | '\u0B83'
 | [\u0B85-\u0B8A]
 | [\u0B8E-\u0B90]
 | [\u0B92-\u0B95]
 | [\u0B99-\u0B9A]
 | '\u0B9C'
 | [\u0B9E-\u0B9F]
 | [\u0BA3-\u0BA4]
 | [\u0BA8-\u0BAA]
 | [\u0BAE-\u0BB9]
 | [\u0C05-\u0C0C]
 | [\u0C0E-\u0C10]
 | [\u0C12-\u0C28]
 | [\u0C2A-\u0C33]
 | [\u0C35-\u0C39]
 | [\u0C60-\u0C61]
 | [\u0C85-\u0C8C]
 | [\u0C8E-\u0C90]
 | [\u0C92-\u0CA8]
 | [\u0CAA-\u0CB3]
 | [\u0CB5-\u0CB9]
 | '\u0CBD'
 | '\u0CDE'
 | [\u0CE0-\u0CE1]
 | [\u0D05-\u0D0C]
 | [\u0D0E-\u0D10]
 | [\u0D12-\u0D28]
 | [\u0D2A-\u0D39]
 | [\u0D60-\u0D61]
 | [\u0D85-\u0D96]
 | [\u0D9A-\u0DB1]
 | [\u0DB3-\u0DBB]
 | '\u0DBD'
 | [\u0DC0-\u0DC6]
 | [\u0E01-\u0E30]
 | [\u0E32-\u0E33]
 | [\u0E40-\u0E45]
 | '\u0E46'
 | [\u0E81-\u0E82]
 | '\u0E84'
 | [\u0E87-\u0E88]
 | '\u0E8A'
 | '\u0E8D'
 | [\u0E94-\u0E97]
 | [\u0E99-\u0E9F]
 | [\u0EA1-\u0EA3]
 | '\u0EA5'
 | '\u0EA7'
 | [\u0EAA-\u0EAB]
 | [\u0EAD-\u0EB0]
 | [\u0EB2-\u0EB3]
 | '\u0EBD'
 | [\u0EC0-\u0EC4]
 | '\u0EC6'
 | [\u0EDC-\u0EDD]
 | '\u0F00'
 | [\u0F40-\u0F47]
 | [\u0F49-\u0F6A]
 | [\u0F88-\u0F8B]
 | [\u1000-\u1021]
 | [\u1023-\u1027]
 | [\u1029-\u102A]
 | [\u1050-\u1055]
 | [\u10A0-\u10C5]
 | [\u10D0-\u10FA]
 | '\u10FC'
 | [\u1100-\u1159]
 | [\u115F-\u11A2]
 | [\u11A8-\u11F9]
 | [\u1200-\u1248]
 | [\u124A-\u124D]
 | [\u1250-\u1256]
 | '\u1258'
 | [\u125A-\u125D]
 | [\u1260-\u1288]
 | [\u128A-\u128D]
 | [\u1290-\u12B0]
 | [\u12B2-\u12B5]
 | [\u12B8-\u12BE]
 | '\u12C0'
 | [\u12C2-\u12C5]
 | [\u12C8-\u12D6]
 | [\u12D8-\u1310]
 | [\u1312-\u1315]
 | [\u1318-\u135A]
 | [\u1380-\u138F]
 | [\u13A0-\u13F4]
 | [\u1401-\u166C]
 | [\u166F-\u1676]
 | [\u1681-\u169A]
 | [\u16A0-\u16EA]
 | [\u16EE-\u16F0]
 | [\u1700-\u170C]
 | [\u170E-\u1711]
 | [\u1720-\u1731]
 | [\u1740-\u1751]
 | [\u1760-\u176C]
 | [\u176E-\u1770]
 | [\u1780-\u17B3]
 | '\u17D7'
 | '\u17DC'
 | [\u1820-\u1842]
 | '\u1843'
 | [\u1844-\u1877]
 | [\u1880-\u18A8]
 | [\u1900-\u191C]
 | [\u1950-\u196D]
 | [\u1970-\u1974]
 | [\u1980-\u19A9]
 | [\u19C1-\u19C7]
 | [\u1A00-\u1A16]
 | [\u1D00-\u1D2B]
 | [\u1D2C-\u1D61]
 | [\u1D62-\u1D77]
 | '\u1D78'
 | [\u1D79-\u1D9A]
 | [\u1D9B-\u1DBF]
 | [\u1E00-\u1E9B]
 | [\u1EA0-\u1EF9]
 | [\u1F00-\u1F15]
 | [\u1F18-\u1F1D]
 | [\u1F20-\u1F45]
 | [\u1F48-\u1F4D]
 | [\u1F50-\u1F57]
 | '\u1F59'
 | '\u1F5B'
 | '\u1F5D'
 | [\u1F5F-\u1F7D]
 | [\u1F80-\u1FB4]
 | [\u1FB6-\u1FBC]
 | '\u1FBE'
 | [\u1FC2-\u1FC4]
 | [\u1FC6-\u1FCC]
 | [\u1FD0-\u1FD3]
 | [\u1FD6-\u1FDB]
 | [\u1FE0-\u1FEC]
 | [\u1FF2-\u1FF4]
 | [\u1FF6-\u1FFC]
 | '\u2071'
 | '\u207F'
 | [\u2090-\u2094]
 | '\u2102'
 | '\u2107'
 | [\u210A-\u2113]
 | '\u2115'
 | '\u2118'
 | [\u2119-\u211D]
 | '\u2124'
 | '\u2126'
 | '\u2128'
 | [\u212A-\u212D]
 | '\u212E'
 | [\u212F-\u2131]
 | [\u2133-\u2134]
 | [\u2135-\u2138]
 | '\u2139'
 | [\u213C-\u213F]
 | [\u2145-\u2149]
 | [\u2160-\u2183]
 | [\u2C00-\u2C2E]
 | [\u2C30-\u2C5E]
 | [\u2C80-\u2CE4]
 | [\u2D00-\u2D25]
 | [\u2D30-\u2D65]
 | '\u2D6F'
 | [\u2D80-\u2D96]
 | [\u2DA0-\u2DA6]
 | [\u2DA8-\u2DAE]
 | [\u2DB0-\u2DB6]
 | [\u2DB8-\u2DBE]
 | [\u2DC0-\u2DC6]
 | [\u2DC8-\u2DCE]
 | [\u2DD0-\u2DD6]
 | [\u2DD8-\u2DDE]
 | '\u3005'
 | '\u3006'
 | '\u3007'
 | [\u3021-\u3029]
 | [\u3031-\u3035]
 | [\u3038-\u303A]
 | '\u303B'
 | '\u303C'
 | [\u3041-\u3096]
 | [\u309B-\u309C]
 | [\u309D-\u309E]
 | '\u309F'
 | [\u30A1-\u30FA]
 | [\u30FC-\u30FE]
 | '\u30FF'
 | [\u3105-\u312C]
 | [\u3131-\u318E]
 | [\u31A0-\u31B7]
 | [\u31F0-\u31FF]
 | [\u3400-\u4DB5]
 | [\u4E00-\u9FBB]
 | [\uA000-\uA014]
 | '\uA015'
 | [\uA016-\uA48C]
 | [\uA800-\uA801]
 | [\uA803-\uA805]
 | [\uA807-\uA80A]
 | [\uA80C-\uA822]
 | [\uAC00-\uD7A3]
 | [\uF900-\uFA2D]
 | [\uFA30-\uFA6A]
 | [\uFA70-\uFAD9]
 | [\uFB00-\uFB06]
 | [\uFB13-\uFB17]
 | '\uFB1D'
 | [\uFB1F-\uFB28]
 | [\uFB2A-\uFB36]
 | [\uFB38-\uFB3C]
 | '\uFB3E'
 | [\uFB40-\uFB41]
 | [\uFB43-\uFB44]
 | [\uFB46-\uFBB1]
 | [\uFBD3-\uFD3D]
 | [\uFD50-\uFD8F]
 | [\uFD92-\uFDC7]
 | [\uFDF0-\uFDFB]
 | [\uFE70-\uFE74]
 | [\uFE76-\uFEFC]
 | [\uFF21-\uFF3A]
 | [\uFF41-\uFF5A]
 | [\uFF66-\uFF6F]
 | '\uFF70'
 | [\uFF71-\uFF9D]
 | [\uFF9E-\uFF9F]
 | [\uFFA0-\uFFBE]
 | [\uFFC2-\uFFC7]
 | [\uFFCA-\uFFCF]
 | [\uFFD2-\uFFD7]
 | [\uFFDA-\uFFDC]
 ;

fragment SYMBOL
 : STARTSYMBOL
 | [0-9]
 | [\u0300-\u036F]
 | [\u0483-\u0486]
 | [\u0591-\u05B9]
 | [\u05BB-\u05BD]
 | '\u05BF'
 | [\u05C1-\u05C2]
 | [\u05C4-\u05C5]
 | '\u05C7'
 | [\u0610-\u0615]
 | [\u064B-\u065E]
 | [\u0660-\u0669]
 | '\u0670'
 | [\u06D6-\u06DC]
 | [\u06DF-\u06E4]
 | [\u06E7-\u06E8]
 | [\u06EA-\u06ED]
 | [\u06F0-\u06F9]
 | '\u0711'
 | [\u0730-\u074A]
 | [\u07A6-\u07B0]
 | [\u0901-\u0902]
 | '\u0903'
 | '\u093C'
 | [\u093E-\u0940]
 | [\u0941-\u0948]
 | [\u0949-\u094C]
 | '\u094D'
 | [\u0951-\u0954]
 | [\u0962-\u0963]
 | [\u0966-\u096F]
 | '\u0981'
 | [\u0982-\u0983]
 | '\u09BC'
 | [\u09BE-\u09C0]
 | [\u09C1-\u09C4]
 | [\u09C7-\u09C8]
 | [\u09CB-\u09CC]
 | '\u09CD'
 | '\u09D7'
 | [\u09E2-\u09E3]
 | [\u09E6-\u09EF]
 | [\u0A01-\u0A02]
 | '\u0A03'
 | '\u0A3C'
 | [\u0A3E-\u0A40]
 | [\u0A41-\u0A42]
 | [\u0A47-\u0A48]
 | [\u0A4B-\u0A4D]
 | [\u0A66-\u0A6F]
 | [\u0A70-\u0A71]
 | [\u0A81-\u0A82]
 | '\u0A83'
 | '\u0ABC'
 | [\u0ABE-\u0AC0]
 | [\u0AC1-\u0AC5]
 | [\u0AC7-\u0AC8]
 | '\u0AC9'
 | [\u0ACB-\u0ACC]
 | '\u0ACD'
 | [\u0AE2-\u0AE3]
 | [\u0AE6-\u0AEF]
 | '\u0B01'
 | [\u0B02-\u0B03]
 | '\u0B3C'
 | '\u0B3E'
 | '\u0B3F'
 | '\u0B40'
 | [\u0B41-\u0B43]
 | [\u0B47-\u0B48]
 | [\u0B4B-\u0B4C]
 | '\u0B4D'
 | '\u0B56'
 | '\u0B57'
 | [\u0B66-\u0B6F]
 | '\u0B82'
 | [\u0BBE-\u0BBF]
 | '\u0BC0'
 | [\u0BC1-\u0BC2]
 | [\u0BC6-\u0BC8]
 | [\u0BCA-\u0BCC]
 | '\u0BCD'
 | '\u0BD7'
 | [\u0BE6-\u0BEF]
 | [\u0C01-\u0C03]
 | [\u0C3E-\u0C40]
 | [\u0C41-\u0C44]
 | [\u0C46-\u0C48]
 | [\u0C4A-\u0C4D]
 | [\u0C55-\u0C56]
 | [\u0C66-\u0C6F]
 | [\u0C82-\u0C83]
 | '\u0CBC'
 | '\u0CBE'
 | '\u0CBF'
 | [\u0CC0-\u0CC4]
 | '\u0CC6'
 | [\u0CC7-\u0CC8]
 | [\u0CCA-\u0CCB]
 | [\u0CCC-\u0CCD]
 | [\u0CD5-\u0CD6]
 | [\u0CE6-\u0CEF]
 | [\u0D02-\u0D03]
 | [\u0D3E-\u0D40]
 | [\u0D41-\u0D43]
 | [\u0D46-\u0D48]
 | [\u0D4A-\u0D4C]
 | '\u0D4D'
 | '\u0D57'
 | [\u0D66-\u0D6F]
 | [\u0D82-\u0D83]
 | '\u0DCA'
 | [\u0DCF-\u0DD1]
 | [\u0DD2-\u0DD4]
 | '\u0DD6'
 | [\u0DD8-\u0DDF]
 | [\u0DF2-\u0DF3]
 | '\u0E31'
 | [\u0E34-\u0E3A]
 | [\u0E47-\u0E4E]
 | [\u0E50-\u0E59]
 | '\u0EB1'
 | [\u0EB4-\u0EB9]
 | [\u0EBB-\u0EBC]
 | [\u0EC8-\u0ECD]
 | [\u0ED0-\u0ED9]
 | [\u0F18-\u0F19]
 | [\u0F20-\u0F29]
 | '\u0F35'
 | '\u0F37'
 | '\u0F39'
 | [\u0F3E-\u0F3F]
 | [\u0F71-\u0F7E]
 | '\u0F7F'
 | [\u0F80-\u0F84]
 | [\u0F86-\u0F87]
 | [\u0F90-\u0F97]
 | [\u0F99-\u0FBC]
 | '\u0FC6'
 | '\u102C'
 | [\u102D-\u1030]
 | '\u1031'
 | '\u1032'
 | [\u1036-\u1037]
 | '\u1038'
 | '\u1039'
 | [\u1040-\u1049]
 | [\u1056-\u1057]
 | [\u1058-\u1059]
 | '\u135F'
 | [\u1369-\u1371]
 | [\u1712-\u1714]
 | [\u1732-\u1734]
 | [\u1752-\u1753]
 | [\u1772-\u1773]
 | '\u17B6'
 | [\u17B7-\u17BD]
 | [\u17BE-\u17C5]
 | '\u17C6'
 | [\u17C7-\u17C8]
 | [\u17C9-\u17D3]
 | '\u17DD'
 | [\u17E0-\u17E9]
 | [\u180B-\u180D]
 | [\u1810-\u1819]
 | '\u18A9'
 | [\u1920-\u1922]
 | [\u1923-\u1926]
 | [\u1927-\u1928]
 | [\u1929-\u192B]
 | [\u1930-\u1931]
 | '\u1932'
 | [\u1933-\u1938]
 | [\u1939-\u193B]
 | [\u1946-\u194F]
 | [\u19B0-\u19C0]
 | [\u19C8-\u19C9]
 | [\u19D0-\u19D9]
 | [\u1A17-\u1A18]
 | [\u1A19-\u1A1B]
 | [\u1DC0-\u1DC3]
 | [\u203F-\u2040]
 | '\u2054'
 | [\u20D0-\u20DC]
 | '\u20E1'
 | [\u20E5-\u20EB]
 | [\u302A-\u302F]
 | [\u3099-\u309A]
 | '\uA802'
 | '\uA806'
 | '\uA80B'
 | [\uA823-\uA824]
 | [\uA825-\uA826]
 | '\uA827'
 | '\uFB1E'
 | [\uFE00-\uFE0F]
 | [\uFE20-\uFE23]
 | [\uFE33-\uFE34]
 | [\uFE4D-\uFE4F]
 | [\uFF10-\uFF19]
 | '\uFF3F'
 ;
