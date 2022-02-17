using Common;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class FireStoreDataAccess
    {
        private FirestoreDb db { get; set; }
        public FireStoreDataAccess(string _projectId)
        {
            db = FirestoreDb.Create(_projectId);
        }
        public async Task<User> GetUser(string email)
        {
            DocumentReference docRef = db.Collection("users").Document(email);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {

                //Console.WriteLine("Document data for {0} document:", snapshot.Id);
                // Dictionary<string, object> city = snapshot.ToDictionary();
                //foreach (KeyValuePair<string, object> pair in city)
                //{
                //    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                //}
                return snapshot.ConvertTo<User>();
            }
            else
            {
                return null;
            }
        }

        public async Task<WriteResult> AddUser(User aNewUser)
        {
            DocumentReference docRef = db.Collection("users").Document(aNewUser.Email);
           return await docRef.SetAsync(aNewUser);
        }

        public void AddMessage(string email, Message msg)
        {

        }

        public List<Message> SearchMessages(string user, string keyword)
        {
            return null;
        }

        public void UpdateUser(User updatedUser)
        { }

        public void DeleteUser(string email)
        { }
    }
}
