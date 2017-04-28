#!/bin/bash
for fn in $(ls *.crawl);  do
  echo "\n"  | ../../src/compilerconsolehost/bin/Debug/compilerconsolehost.exe -a --force-single-thread $fn > /dev/null; 
  echo ;
  echo $fn ;
  echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━;
done 
