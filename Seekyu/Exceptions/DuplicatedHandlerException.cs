using Seekyu.Extensions;
using System;

namespace Seekyu.Exceptions
{
    public class DuplicatedHandlerException : Exception
    {
        private readonly string DispatchableTypeName;
        private readonly string ResultTypeName;

        public override string Message => $"There is already a Handler of <{DispatchableTypeName}, {ResultTypeName}> registered";

        public DuplicatedHandlerException(Type tDispatchable, Type tResult)
        {
            DispatchableTypeName = tDispatchable.ToGenericTypeString();
            ResultTypeName = tResult.ToGenericTypeString();
        }
    }
}
