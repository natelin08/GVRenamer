using GVRenamer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GVRenamer.Extensions
{
    public static class StringExtensions
    {
        public static string ToRuleName(this string value, VideoModel videoModel)
        {
            return value.Replace("%number%", videoModel.Number).Replace("%studio%", videoModel.Studio).Replace("%title%", videoModel.Title);
        }
        public static string TrimIgnoreFileName(this string value)
        {
            return value.Replace("  ", " ").Replace(":", "-").TrimStart(' ').TrimEnd(' ');
        }
    }
}
