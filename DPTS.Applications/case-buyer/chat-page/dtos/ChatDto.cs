using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Applications.case_buyer.chat_page.dtos
{
    public class ChatDto
    {
        public ChatInfo ChatInfo { get; set; }
        public IEnumerable<MessageDto> MessageDtos { get; set; }
    }
    public record ChatInfo
    {
        public string Image { get; set; } 
        public string Name { get; set; }
        public string Id { get; set; }
    }
    public record MessageDto
    {
        public string Content { get; set; }
        public string SendAt { get; set; }
    }
}
