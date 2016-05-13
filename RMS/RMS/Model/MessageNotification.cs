using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.UI.Model
{
    public class MessageNotification : IComparer<MessageNotification>
    {
        private int _messageId;

        public int MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }
        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public MessageNotification()
        { }

        public MessageNotification(int id, string msg) : this()
        {
            _messageId = id;
            _message = msg;
            _time = DateTime.Now;
        }

        DateTime _time;
        public DateTime Time
        {
            get { return _time; }
        }

        public int Compare(MessageNotification x, MessageNotification y)
        {
            int compareDate = y.Time.CompareTo(x.Time);
            
            return compareDate;
        }
    }
}
