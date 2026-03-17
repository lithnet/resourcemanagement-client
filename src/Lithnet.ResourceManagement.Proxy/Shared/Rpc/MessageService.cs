using System.IO;
using System.ServiceModel.Channels;
using System.Xml;

namespace Lithnet.ResourceManagement.Proxy
{
    public static class MessageService
    {
        public static string Serialize(Message input)
        {
            StringWriter ms = new StringWriter();
            XmlWriter w = XmlWriter.Create(ms);
            input.WriteMessage(w);
            w.Flush();

            return ms.ToString();
        }

        public static Message Deserialize(string input)
        {
            XmlReader r = XmlReader.Create(new StringReader(input));
            return Message.CreateMessage(r, int.MaxValue, MessageVersion.Default);
        }
    }
}