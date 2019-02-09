using System;
using System.Linq;
using System.Threading.Tasks;

namespace MicroElements.Functional.Tests.Domain
{
    public class SampleParser
    {
        public Result<ParsedObject, Exception, string> Parse(string source)
        {
            ParsedObject parsedObject = new ParsedObject();

            IMessageList<string> errors = MessageList<string>.Empty;
            IMessageList<string> messageList = MessageList<string>.Empty;

            ParseA(source)
                .MatchSuccess((a, list) => parsedObject.A = a)
                .MatchError((e, list) => errors = errors.AddRange(list))
                .MatchMessages(list => messageList = messageList.AddRange(list));

            ParseB(source)
                .MatchSuccess((b, list) => parsedObject.B = b)
                .MatchError((e, list) => errors = errors.AddRange(list))
                .MatchMessages(list => messageList = messageList.AddRange(list));

            return errors.Count == 0
                ? parsedObject.ToSuccess(messageList)
                : messageList.ToFail<ParsedObject, Exception, string>();
        }

        public Result<int, Exception, string> ParseA(string source)
        {
            var src = source.Split(";").FirstOrDefault();
            return Prelude.ParseInt(src)
                .Match(
                    some: i => i.ToSuccess("A parsed"),
                    none:() => $"{src} can not be parsed to int");
        }

        public Result<string, Exception, string> ParseB(string source)
        {
            var src = source.Split(";").LastOrDefault();
            return src.ToSuccess("B parsed");
        }

        public async Task<Result<string, Exception, string>> ParseBAsync(string source)
        {
            await Task.Delay(1);
            return ParseB(source);
        }
    }
}
