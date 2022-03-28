using Common;
using DataAccess;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63A2022.Controllers
{
    public class UsersController : Controller
    {
        private readonly FireStoreDataAccess fireStore;
        private readonly PubsubAccess pubsub;
        public UsersController(FireStoreDataAccess _fireStore, PubsubAccess _pubsub)
        {
            pubsub = _pubsub;
            fireStore = _fireStore;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
           Common.User myUser = await fireStore.GetUser(User.Claims.ElementAt(4).Value);
            if (myUser == null)
                return View(new Common.User() { Email = User.Claims.ElementAt(4).Value });
            else return View(myUser);
        }

        [Authorize]
        public async Task<IActionResult> Register(User user)
        {
           user.Email = User.Claims.ElementAt(4).Value;
           await fireStore.AddUser(user);
            
           return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Send()
        { return View();
        }

        [Authorize][HttpPost]
        public async Task<IActionResult> Send(Message msg, IFormFile attachment)
        {
            string bucketName = "swd63a2022ra"; //store it in appsettings.json
            msg.Id = Guid.NewGuid().ToString();

            if(attachment != null)
            {
                    //1. upload file on bucket

                var storage = StorageClient.Create();

                using (Stream myStream = attachment.OpenReadStream())
                {
                     storage.UploadObject(bucketName, msg.Id +Path.GetExtension(attachment.FileName) , null, myStream);
                }

                //1.5 only for Fine Grained type of buckets 

                var storageObject = storage.GetObject(bucketName, msg.Id + Path.GetExtension(attachment.FileName), new GetObjectOptions
                {
                    Projection = Projection.Full
                });

                storageObject.Acl.Add(new ObjectAccessControl
                {
                    Bucket = bucketName,
                    Entity = $"user-{User.Claims.ElementAt(4).Value}",
                    Role = "OWNER",
                });

                storageObject.Acl.Add(new ObjectAccessControl
                {
                    Bucket = bucketName,
                    Entity = $"user-{msg.Recipient}",
                    Role = "READER",
                });

                var updatedObject = storage.UpdateObject(storageObject);


                //2. set msg.AttachmentUri to the uri to download it

                //if bucket is uniform with public access: "https://storage.googleapis.com/{bucketName}/{filename}"
                //if bucket is fine grained with dedicated acl on objects: "https://storage.cloud.google.com/{bucketName}/{filename}"


                //msg.AttachmentUri = $"https://storage.googleapis.com/{bucketName}/{ msg.Id + Path.GetExtension(attachment.FileName)}";

                msg.AttachmentUri = $"https://storage.cloud.google.com/{bucketName}/{ msg.Id + Path.GetExtension(attachment.FileName)}";
            }

           // msg.DateSent = Google.Cloud.Firestore.Timestamp.FromDateTime(DateTime.UtcNow);
            await fireStore.AddMessage(User.Claims.ElementAt(4).Value, msg);

            
            //adding the message info into the queue so later on, it can be sent via email
            await pubsub.Publish(msg);


            return RedirectToAction("List");
        }
        [Authorize]
        public async Task<IActionResult> List()
        {
            var messages = await fireStore.ListMessages(User.Claims.ElementAt(4).Value);
            return View(messages);
        }
    }
}
