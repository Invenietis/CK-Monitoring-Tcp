using CK.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Monitoring
{
    public static class LogExtensions
    {
        public static string ToIndexableString( this DateTimeStamp @this )
        {
            // https://lucene.apache.org/core/3_0_3/api/core/org/apache/lucene/document/DateTools.html
            // yyyyMMddHHmmssSSS 
            return $"{@this.TimeUtc.ToString( @"yyyyMMddHHmmssfffffff" )}{@this.Uniquifier:D3}";
        }
        public static string ToIndexableString( this CKTrait @this )
        {
            return string.Join( "; ", @this.AtomicTraits.Select( x => x.ToString() ) );
        }
        public static string ToIndexableString( this CKExceptionData @this )
        {
            return @this.ToString();
        }
        public static string ToIndexableString( this IReadOnlyList<ActivityLogGroupConclusion> @this )
        {
            return string.Join( "; ", @this.Select( x => x.Text ) );
        }

    }
}
