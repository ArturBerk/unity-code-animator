using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnimator
{
    public sealed class StringBuilderText : SourceText
    {
        /// <summary>
        /// Underlying string on which this SourceText instance is based
        /// </summary>
        private readonly StringBuilder _builder;

        private readonly Encoding _encodingOpt;

        public StringBuilderText(
            StringBuilder builder,
            Encoding encodingOpt,
            SourceHashAlgorithm checksumAlgorithm)
            : base(new ImmutableArray<byte>(), checksumAlgorithm, (SourceTextContainer) null)
        {
            this._builder = builder;
            this._encodingOpt = encodingOpt;
        }

        public override Encoding Encoding
        {
            get { return this._encodingOpt; }
        }

        /// <summary>
        /// Underlying string which is the source of this SourceText instance
        /// </summary>
        internal StringBuilder Builder
        {
            get { return this._builder; }
        }

        /// <summary>
        /// The length of the text represented by <see cref="T:Microsoft.CodeAnalysis.Text.StringBuilderText" />.
        /// </summary>
        public override int Length
        {
            get { return this._builder.Length; }
        }

        /// <summary>Returns a character at given position.</summary>
        /// <param name="position">The position to get the character from.</param>
        /// <returns>The character.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">When position is negative or
        /// greater than <see cref="P:Microsoft.CodeAnalysis.Text.StringBuilderText.Length" />.</exception>
        public override char this[int position]
        {
            get
            {
                if (position < 0 || position >= this._builder.Length)
                    throw new ArgumentOutOfRangeException(nameof(position));
                return this._builder[position];
            }
        }

        /// <summary>
        /// Provides a string representation of the StringBuilderText located within given span.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">When given span is outside of the text range.</exception>
        public override string ToString(TextSpan span)
        {
            if (span.End > this._builder.Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            return this._builder.ToString(span.Start, span.Length);
        }

        public override void CopyTo(
            int sourceIndex,
            char[] destination,
            int destinationIndex,
            int count)
        {
            this._builder.CopyTo(sourceIndex, destination, destinationIndex, count);
        }
    }
}