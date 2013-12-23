namespace OpusWrapper.Opus.Presets
{
    public sealed class ChanelTemplateCollection
    {
        public struct ChanelTemplate
        {
            /// <summary>
            ///Number of streams
            /// </summary>
            public byte StreamsCount;

            //Number of oupled streams
            public byte CoupledStreamsCount;

            /// <summary>
            /// Chanel mapping
            /// </summary>
            public byte[] Mapping;
        }

        private static readonly ChanelTemplate[] Items;

        static ChanelTemplateCollection()
        {
            Items = new[]
                {
                    //1: mono
                    new ChanelTemplate {StreamsCount = 1, CoupledStreamsCount = 0, Mapping = new byte[] {0}},
                    //2: stereo
                    new ChanelTemplate {StreamsCount = 1, CoupledStreamsCount = 1, Mapping = new byte[] {0, 1}},
                    //3: 1-d surround
                    new ChanelTemplate {StreamsCount = 2, CoupledStreamsCount = 1, Mapping = new byte[] {0, 2, 1}},
                    //4: quadraphonic surround               
                    new ChanelTemplate {StreamsCount = 2, CoupledStreamsCount = 2, Mapping = new byte[] {0, 1, 2, 3}},
                    //5: 5-channel surround
                    new ChanelTemplate {StreamsCount = 3, CoupledStreamsCount = 2, Mapping = new byte[] {0, 4, 1, 2, 3}},
                    //6: 5.1 surround
                    new ChanelTemplate {StreamsCount = 4, CoupledStreamsCount = 2, Mapping = new byte[] {0, 4, 1, 2, 3, 5}},
                    //7: 6.1 surround
                    new ChanelTemplate {StreamsCount = 4, CoupledStreamsCount = 3, Mapping = new byte[] {0, 4, 1, 2, 3, 5, 6}},
                    //8: 7.1 surround
                    new ChanelTemplate
                        {
                            StreamsCount = 5,
                            CoupledStreamsCount = 3,
                            Mapping = new byte[] {0, 6, 1, 2, 3, 4, 5, 7}
                        }
                };
        }

        public static ChanelTemplate GetTemplate(int chanelCount)
        {
            return Items[chanelCount];
        }
    };
}