using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string LastName { get; set; }
        [FirestoreProperty]
        public string ProfilePicUrl { get; set; }
        [FirestoreProperty]
        public string Mobile { get; set; }

        public string FullName
        {
            get { return Name + " " + LastName; }
        }

       
        public List<Message> Messages { get; set; }


    }
}
