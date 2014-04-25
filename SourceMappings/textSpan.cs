using System;
using System.Diagnostics;

namespace TypeScript 
{
    public interface ISpan 
    {
        int start();
        int end();
    }

    public class TextSpan : ISpan 
    {
        private int _start;
        private int _length;

        /**
         * Creates a TextSpan instance beginning with the position Start and having the Length
         * specified with length.
         */
        TextSpan(int start, int length) {
            if (start < 0) {
                Errors.argument("start");
            }

            if (length < 0) {
                Errors.argument("length");
            }

            this._start = start;
            this._length = length;
        }

        public int start() {
            return this._start;
        }

        public int length() {
            return this._length;
        }

        public int end() {
            return this._start + this._length;
        }

        public bool isEmpty() {
            return this._length == 0;
        }

        /**
         * Determines whether the position lies within the span. Returns true if the position is greater than or equal to Start and strictly less 
         * than End, otherwise false.
         * @param position The position to check.
         */
        public bool containsPosition(int position) {
            return position >= this._start && position < this.end();
        }

        /**
         * Determines whether span falls completely within this span. Returns true if the specified span falls completely within this span, otherwise false.
         * @param span The span to check.
         */
        public bool containsTextSpan(TextSpan span) {
            return span._start >= this._start && span.end() <= this.end();
        }

        /**
         * Determines whether the given span overlaps this span. Two spans are considered to overlap 
         * if they have positions in common and neither is empty. Empty spans do not overlap with any 
         * other span. Returns true if the spans overlap, false otherwise.
         * @param span The span to check.
         */
        public bool overlapsWith(TextSpan span) {
            var overlapStart = Math.Max(this._start, span._start);
            var overlapEnd = Math.Min(this.end(), span.end());

            return overlapStart < overlapEnd;
        }

        /**
         * Returns the overlap with the given span, or null if there is no overlap.
         * @param span The span to check.
         */
        public TextSpan overlap(TextSpan span) {
            var overlapStart = Math.Max(this._start, span._start);
            var overlapEnd = Math.Min(this.end(), span.end());

            if (overlapStart < overlapEnd) {
                return TextSpan.fromBounds(overlapStart, overlapEnd);
            }

            return null;
        }

        /**
         * Determines whether span intersects this span. Two spans are considered to 
         * intersect if they have positions in common or the end of one span 
         * coincides with the start of the other span. Returns true if the spans intersect, false otherwise.
         * @param The span to check.
         */
        public bool intersectsWithTextSpan(TextSpan span) {
            return span._start <= this.end() && span.end() >= this._start;
        }

        public bool intersectsWith(int start, int length) {
            var end = start + length;
            return start <= this.end() && end >= this._start;
        }

        /**
         * Determines whether the given position intersects this span. 
         * A position is considered to intersect if it is between the start and
         * end positions (inclusive) of this span. Returns true if the position intersects, false otherwise.
         * @param position The position to check.
         */
        public bool intersectsWithPosition(int position) {
            return position <= this.end() && position >= this._start;
        }

        /**
         * Returns the intersection with the given span, or null if there is no intersection.
         * @param span The span to check.
         */
        public TextSpan intersection(TextSpan span) {
            var intersectStart = Math.Max(this._start, span._start);
            var intersectEnd = Math.Min(this.end(), span.end());

            if (intersectStart <= intersectEnd) {
                return TextSpan.fromBounds(intersectStart, intersectEnd);
            }

            return null;
        }

        /**
         * Creates a new TextSpan from the given start and end positions
         * as opposed to a position and length.
         */
        public static TextSpan fromBounds(int start, int end) {
            Debug.Assert(start >= 0);
            Debug.Assert(end - start >= 0);
            return new TextSpan(start, end - start);
        }
    }
}