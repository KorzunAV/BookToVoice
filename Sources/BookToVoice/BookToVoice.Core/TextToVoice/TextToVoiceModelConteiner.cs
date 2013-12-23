using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace BookToVoice.Core.TextToVoice
{
    [XmlRoot("TextToVoiceModelConteiner")]
    [XmlInclude(typeof(TextToVoiceModel))]
    public class TextToVoiceModelConteiner : ObservableCollection<TextToVoiceModel>
    {
    }
}
