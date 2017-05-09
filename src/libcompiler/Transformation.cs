using System;
using System.Collections.Concurrent;

namespace libcompiler
{
    public class Transformation
    {
        public static TransformationStart<TIn, TExtra> From<TIn, TExtra>(ConcurrentBag<TIn> input, TExtra extra)
        {
            return new TransformationStart<TIn, TExtra>(input, extra);
        }

        public class TransformationStart<TData, TExtra>
        {
            protected readonly ConcurrentBag<TData> _input;
            protected readonly TExtra _extra;

            internal TransformationStart(ConcurrentBag<TData> input, TExtra extra)
            {
                _input = input;
                _extra = extra;
            }

            public TransformationStage<TData, TNext, TExtra> With<TNext>(Func<TData, TExtra, TNext> transformation)
            {
                return new TransformationStage<TData, TNext, TExtra>(this, transformation);
            }
        }

        public class TransformationStage<TStart, TEnd, TExtra>
        {
            private readonly TransformationStart<TStart,TExtra> _input;
            private readonly Func<TStart, TExtra, TEnd> _transfomration;

            internal TransformationStage(TransformationStart<TStart, TExtra> input, Func<TStart, TExtra, TEnd> transfomration)
            {
                _input = input;
                _transfomration = transfomration;
            }

            public TransformationStage<TStart, TNewEnd, TExtra> With<TNewEnd>(Func<TEnd, TExtra, TNewEnd> nextTransformation)
            {
                return new TransformationStage<TStart, TNewEnd, TExtra>(
                    _input,
                    ((start, extra) => nextTransformation(_transfomration(start, extra), extra)));
            }

            public ConcurrentBag<TEnd> Execute(bool parallel)
            {
                ConcurrentBag<TEnd> destination = new ConcurrentBag<TEnd>();
                foreach (var item in _input._input)
                {
                    destination.Add(_transfomration(item, _input._extra));
                }

                return destination;
            }
        }
    }


}