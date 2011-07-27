using System;

namespace NodaTime.Format.Builder
{
    internal interface IFormatterBuilderNode<in TInfo> where TInfo : ParseInfo
    {
        IFormatNode<TInfo> MakeNode(IFormatProvider formatProvider);
    }
}