using System;
using System.Collections.Generic;
using System.Linq;
using OpusWrapper.Opus.Presets;

namespace OpusWrapper.Opus
{
    public class OpusTags
    {
        private readonly int _userCommentListLengthPosition;
        private readonly List<byte> _data;
        private bool _firtEncoderOptions = true;

        public OpusTags()
        {
            var vendor = Api.opus_get_version_string();
            _data = new List<byte>(8 + 4 + vendor.Length + 4);

            AddString("OpusTags");
            AddInt32(vendor.Length);
            AddString(vendor);
            _userCommentListLengthPosition = _data.Count;
            AddInt32(0); //user comment list length
        }

        private void AddString(string value)
        {
            foreach (var t in value)
            {
                _data.Add((byte)t);
            }
        }

        private void AddInt32(int value)
        {
            var bvendorLen = BitConverter.GetBytes(value);
            _data.AddRange(bvendorLen);
        }

        public void Add(string tag, string val)
        {
            AddInt32(tag.Length + 1 + val.Length);
            AddString(tag + "=" + val);

            var commentListLength = _data.Skip(_userCommentListLengthPosition).Take(4).ToArray();
            int userCommentListLength = BitConverter.ToInt32(commentListLength, 0);
            userCommentListLength++;
            var newLenBytes = BitConverter.GetBytes(userCommentListLength);
            for (int i = 0; i < newLenBytes.Length; i++)
            {
                _data[_userCommentListLengthPosition + i] = newLenBytes[i];
            }
        }

        public void AddOption<T>(BaseOption<T> option)
        {
            AddOption(option.OptionName, option.InfoValue);
        }

        public void AddOption(string tag, string val)
        {
            var line = string.Format("--{0}{1}{2}", tag, string.IsNullOrEmpty(val) ? string.Empty : " ", val);
            if (_firtEncoderOptions)
            {
                AddString(string.Format("ENCODER_OPTIONS={0}", line));
                _firtEncoderOptions = false;
            }
            AddString(string.Format(" {0}", line));
        }

        public void Pad(int amount = 512)
        {
            if (amount > 0)
            {
                /*Make sure there is at least amount worth of padding free, and
                   round up to the maximum that fits in the current ogg segments.*/
                var newlen = (_data.Count + amount + 255) / 255 * 255 - 1 - _data.Count;

                for (int i = 0; i < newlen; i++)
                {
                    _data.Add(0);
                }
            }
        }

        public byte[] GetPacked()
        {
            return _data.ToArray();
        }

        public int GetPackedLength()
        {
            return _data.Count;
        }
    }
}