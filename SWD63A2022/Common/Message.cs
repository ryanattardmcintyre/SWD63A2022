using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [FirestoreData]
    public class Message
    {
        [FirestoreProperty, ServerTimestamp]
        public Google.Cloud.Firestore.Timestamp DateSent { get; set; }
        [FirestoreProperty]
        public string Text { get; set; }
        [FirestoreProperty]
        public string Recipient { get; set; }
        [FirestoreProperty]
        public string Id { get; set; }
    }
}
