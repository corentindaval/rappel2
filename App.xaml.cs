
#if ANDROID
using Android.Content;
using Android.OS;
using rappel.Platforms.Android;
#endif
using Microsoft.Maui.Graphics.Converters;
using rappel.models;
using System.ComponentModel;
using System.Data;
using rappel.pages;


namespace rappel
{
    public partial class App : Application
    {
        public DataService datas = new DataService { };
        public List<domicile> listdoms;
        public int delai = 5;
        public string datatest;

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
#if ANDROID
            // Inscrire l'événement pour recevoir les données du service
            Platforms.Android.MyBroadcastReceiver.OnDataReceived += OnServiceDataReceived;

            // Enregistrer le BroadcastReceiver
            var filter = new Android.Content.IntentFilter("com.maui.backgroundtask.RESPONSE");
            Android.App.Application.Context.RegisterReceiver(new Platforms.Android.MyBroadcastReceiver(), filter);
#endif
        }

        protected override void OnStart()
        {
            base.OnStart();
            charg();
            verifauth();
            if (datas.activer == true && datas.listdoms!=null) {
                OnStartServiceClicked();
                OnSendToServiceClicked(0);
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
            charg();
        }

        public async Task<PermissionStatus> CheckAndRequestcontactPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.ContactsRead>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }
            status = await Permissions.RequestAsync<Permissions.ContactsRead>();
            return status;
        }


        public async Task<PermissionStatus> CheckAndRequestSMSPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Sms>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.Sms>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }
            status = await Permissions.RequestAsync<Permissions.Sms>();
            return status;
        }

        public async Task<PermissionStatus> CheckAndRequestGPSPermissionPerm()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationAlways>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }
            status = await Permissions.RequestAsync<Permissions.LocationAlways>();
            return status;
        }

        public async Task<PermissionStatus> CheckAndRequestGPSPermissionTemp()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return status;
        }

        public void OnStartServiceClicked()
        {
            // Démarrer le service Android
#if ANDROID
            var intent = new Android.Content.Intent(Android.App.Application.Context, typeof(MyForegroundService));
            Android.App.Application.Context.StartForegroundService(intent);
#endif
        }

        public void StopService()
        {
#if ANDROID
            var intent = new Android.Content.Intent(Android.App.Application.Context, typeof(MyForegroundService));
            intent.SetAction("STOP_SERVICE");
            Android.App.Application.Context.StartForegroundService(intent);
#endif
        }


        //function maj interface
        public void OnDataReceived(string data)
        {
            // Mettre à jour l'interface utilisateur avec les données reçues
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // DataLabel.Text = data;
            });
        }


        //function reponse du service
        public void OnServiceDataReceived(string data)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                string domatteint = data;
                if (domatteint != null)
                {
                    if (domatteint == "horsdom")
                    {
                        if (datas.adom.ToString() == "True")
                        {
                            setderindexmessage();
                            datas.adom = false;
                        }
                    }
                    else
                    {
                        if (datas.adom == false)
                        {
                            recupnvmsg(datas.derindex);
                            MessagingCenter.Send(this, "majnvmsg");
                            datas.adom = true;
                        }
                    }
                }
            });
        }

        public void setderindexmessage()
        {
            if (((App)Application.Current).datas.permsms.Equals(PermissionStatus.Granted))
            {
#if ANDROID
                var service = DependencyService.Get<IMmsService>();
                if (service != null)
                {
                    var mmsMessages = service.GetMmsMessages();
                    ((App)Application.Current).datas.derindexmms = mmsMessages.Count - 1;
                    Preferences.Set("derindexmms", ((App)Application.Current).datas.derindexmms.ToString());
                }
                List<messages> items = new List<messages>();
                string INBOX = "content://sms/inbox";
                string[] reqCols = new string[] { "_id", "thread_id", "address", "person", "date", "body", "type" };
                Android.Net.Uri uri = Android.Net.Uri.Parse(INBOX);
                Android.Database.ICursor cursor = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.ContentResolver.Query(uri, reqCols, null, null, null);

                if (cursor.MoveToFirst())
                {
                    do
                    {
                        var message=new messages();
                        message.estmms = false;
                        message.msgid=int.Parse(cursor.GetString(cursor.GetColumnIndex(reqCols[0])));
                        message.tid=int.Parse(cursor.GetString(cursor.GetColumnIndex(reqCols[1])));
                        message.numero = cursor.GetString(cursor.GetColumnIndex(reqCols[2]));
                        message.name=cursor.GetString(cursor.GetColumnIndex(reqCols[3]));
                        message.date=cursor.GetString(cursor.GetColumnIndex(reqCols[4]));
                        message.msg=cursor.GetString(cursor.GetColumnIndex(reqCols[5]));
                        message.type=cursor.GetString(cursor.GetColumnIndex(reqCols[6]));
                        items.Add(message);
      
                    } while (cursor.MoveToNext());
                    ((App)Application.Current).datas.derindex = items.Count-1;
                    Preferences.Set("derindex", ((App)Application.Current).datas.derindex.ToString());
                }
         
#endif
                }

        }

        public void recupnvmsg(int derindex)
        {
            //function recup message depuis derindex et ajout a datas.listmsg
            if (((App)Application.Current).datas.permsms.Equals(PermissionStatus.Granted))
            {
#if ANDROID
                var nbnvmsg = 0;
                var service = DependencyService.Get<IMmsService>();
                if (service != null)
                {
                    var mmsMessages = service.GetMmsMessages();
                    foreach (var mms in mmsMessages)
                    {
                        if (int.Parse(mms.Id) > datas.derindexmms + 1)
                        {
                            datas.listmms.Add(mms);
                            nbnvmsg += 1;
                        }
                    }
                }
                List<messages> items = new List<messages>();
                string INBOX = "content://sms/inbox";
                string[] reqCols = new string[] { "_id", "thread_id", "address", "person", "date", "body", "type" };
                Android.Net.Uri uri = Android.Net.Uri.Parse(INBOX);
                Android.Database.ICursor cursor = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.ContentResolver.Query(uri, reqCols, null, null, null);
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        var message=new messages();
                        if(int.Parse(cursor.GetString(cursor.GetColumnIndex(reqCols[0])))>derindex+1){
                        nbnvmsg+=1;
                         message.msgid=int.Parse(cursor.GetString(cursor.GetColumnIndex(reqCols[0])));
                        message.tid=int.Parse(cursor.GetString(cursor.GetColumnIndex(reqCols[1])));
                        var nvnum= cursor.GetString(cursor.GetColumnIndex(reqCols[2]));
                        var premchar = nvnum[0];
                        if (premchar != 0)
                        {
                            var npremchar = premchar;
                            var pindex = 0;
                            //trouver index debut puis recup num a partir de l'index
                            for (int i = 0; i <= nvnum.Length-1; i++)
                            {
                                if (nvnum[i].ToString() == "0")
                                {
                                    pindex = i;
                                    break;
                                }
                            }
                            var majnum = "";
                            for(int i=pindex;i<=nvnum.Length-1; i++)
                            {
                                majnum += nvnum[i];
                            }
                            message.numero = majnum;
                        }
                        else
                        {
                            message.numero = cursor.GetString(cursor.GetColumnIndex(reqCols[2]));
                        }
                        message.name=cursor.GetString(cursor.GetColumnIndex(reqCols[3]));
                        message.date=cursor.GetString(cursor.GetColumnIndex(reqCols[4]));
                        message.msg=cursor.GetString(cursor.GetColumnIndex(reqCols[5]));
                        message.type=cursor.GetString(cursor.GetColumnIndex(reqCols[6]));
                        items.Add(message);
                        }
                    } while (cursor.MoveToNext());
                    if(nbnvmsg>0){
                        foreach(messages msg in items)
                        {
                            ((App)Application.Current).datas.listmsg.Add(msg);
                        }  
                    } 
                    savemsg();
                }
                if (nbnvmsg == 0)
                {
                    MainActivity.Instance?.ShowNotification("rappel", "vous n'avez pas de nouveau message");
                }
                else if (nbnvmsg == 1) {
                    MainActivity.Instance?.ShowNotification("rappel", "vous avez 1 nouveau message");
                } else{
                    MainActivity.Instance?.ShowNotification("rappel", "vous avez "+nbnvmsg+" nouveaux messages");
                }
#endif
            }


        }

       

        public void savemsg()
        {
            if (((App)Application.Current).datas.listmsg != null)
            {
                var stocktxt = "";
                var sepdom = " /sd ";
                var sepel = " /sl ";
                var num = 0;
                foreach (var dom in ((App)Application.Current).datas.listmsg)
                {
                    if (num > 0)
                    {
                        stocktxt += sepdom;
                    }
                    else
                    {
                        num += 1;
                    }
                    string msgid = dom.msgid.ToString();
                    string tid = dom.tid.ToString();
                    string nume = dom.numero.ToString();
                    string name = dom.name;
                    string date = dom.date.ToString();
                    string msg = dom.msg;
                    string type = dom.type.ToString();
                    stocktxt += msgid + sepel + tid + sepel + nume + sepel + name + sepel + date + sepel + msg + sepel + type;
                }
                Preferences.Set("listmsg", stocktxt);
            }

        }

        public void savemms()
        {
            if (((App)Application.Current).datas.listmms != null)
            {
                var stocktxt = "";
                var sepdom = " /sd ";
                var sepel = " /sl ";
                var sepmed = "/,/";
                var num = 0;
                foreach (var dom in ((App)Application.Current).datas.listmms)
                {
                    if (num > 0)
                    {
                        stocktxt += sepdom;
                    }
                    else
                    {
                        num += 1;
                    }
                    string id = dom.Id;
                    string text = dom.Text;
                    string lmed = "";
                    var pmed = true;
                    foreach(var med in dom.MediaPaths)
                    {
                        if (pmed == true)
                        {
                            lmed += med;
                            pmed = false;
                        }
                        else
                        {
                            lmed += sepmed + med;
                        }
                    }
                    string exp = dom.Sender;
                    stocktxt += id + sepel + text + sepel + lmed + sepel + exp;
                }
                Preferences.Set("listmms", stocktxt);
            }

        }

        // Function envoie de donnée au service
        public void OnSendToServiceClicked(int mode)
        {
#if ANDROID
            var intent = new Android.Content.Intent(Android.App.Application.Context, typeof(MyForegroundService));
#endif

            if (mode==0 || mode == 1)
            {
                var seploc = "/!/";
                var seplonglat = ",";
                var txtlist = "";
                if (datas.listdoms != null)
                {
                    listdoms = datas.listdoms;
                }
                if (datas.delai != null)
                {
                    delai = datas.delai;
                }
                if (listdoms != null)
                {
                    bool depart = true;

                    foreach (var dom in listdoms)
                    {
                        if (depart == true)
                        {
                            txtlist += dom.latitude.ToString() + seplonglat + dom.longitude.ToString();
                            depart = false;
                        }
                        else
                        {
                            txtlist += seploc + dom.latitude.ToString() + seplonglat + dom.longitude.ToString();
                        }
                    }
                }
                var targetLocation = txtlist;
                if (string.IsNullOrWhiteSpace(targetLocation))
                {
                    //  DisplayAlert("Erreur", "Veuillez entrer une géolocalisation valide.", "OK");
                    return;
                }
             #if ANDROID
                intent.PutExtra("TargetLocation", targetLocation);
             #endif

            }
            if (mode == 0 || mode == 2) {
             #if ANDROID
                intent.PutExtra("delai", convertint(delai));
             #endif
            }  
#if ANDROID
            Android.App.Application.Context.StartForegroundService(intent);
#endif
        }

        public string convertint(int value)
        {
            string res="5";
            string testv = value.ToString();
            if(int.TryParse(testv, out int number))
            {
              //  res = testv;
            }
            else
            {

            }
                return res;
        }

        public async void verifauth()
        {
            var rescont = await CheckAndRequestcontactPermission();
            datas.permcontact = rescont;
            var ressms = await CheckAndRequestSMSPermission();
            datas.permsms = ressms;
            var gpsperm = await CheckAndRequestGPSPermissionPerm();
            datas.permgpsperm = gpsperm;
            var gpstemp = await CheckAndRequestGPSPermissionTemp();
            datas.permgpstemp = gpstemp;
        }

        public void charg()
        {
            if (Preferences.ContainsKey("cback"))
            {
                datas.cback = Color.FromHex(Preferences.Get("cback", ""));
            }
            if (Preferences.ContainsKey("ctxt"))
            {
                datas.ctxt = Color.FromHex(Preferences.Get("ctxt", ""));
            }
            if (Preferences.ContainsKey("ttxt"))
            {
                datas.ttxt = int.Parse(Preferences.Get("ttxt", ""));
            }
                if (Preferences.ContainsKey("active")){
                if (Preferences.Get("active", "") == "True")
                {
                    datas.activer = true;
                }
                else
                {
                    datas.activer = false;
                }
            }
            if (Preferences.ContainsKey("delai"))
            {
                datas.delai = int.Parse(Preferences.Get("delai", ""));
            }
            if (Preferences.ContainsKey("derindex"))
            {
                datas.derindex = int.Parse(Preferences.Get("derindex", ""));
            }
            if (Preferences.ContainsKey("derindexmms"))
            {
                datas.derindexmms = int.Parse(Preferences.Get("derindexmms", ""));
            }
            if (Preferences.ContainsKey("adom"))
            {
                if (Preferences.Get("adom", "") == "true")
                {
                    datas.adom = true;
                }
                else
                {
                   datas.adom = false;
                }
            }
            if (Preferences.ContainsKey("listdom"))
            {
                List<domicile> nvlistdom = new List<domicile>();
                var stringlist = Preferences.Get("listdom", "");
                var sepdom = " /sd ";
                var sepel = " /sl ";
                string[] ldom = stringlist.Split(sepdom);
                foreach (var dom in ldom)
                {
                    string[] lel = dom.Split(sepel);
                    var nvdom = new domicile
                    {
                        nom = lel[0],
                        longitude = double.Parse(lel[1]),
                        latitude = double.Parse(lel[2]),
                        iddomicile = int.Parse(lel[3])
                    };
                    nvlistdom.Add(nvdom);
                }
                datas.listdoms = nvlistdom;
            }
            if (Preferences.ContainsKey("listmms"))
            {
                List<MmsMessage> nvlistmms=new List<MmsMessage>();
                var stringlist = Preferences.Get("listmms", "");
                var sepdom = " /sd ";
                var sepel = " /sl ";
                var sepmed = "/,/";
                string[] lmms = stringlist.Split(sepdom);
                foreach (var dom in lmms)
                {
                    string[] lel = dom.Split(sepel);
                    List<string> nvlmed=new List<string>();
                    string[] lmed = lel[2].Split(sepmed);
                    foreach(var med in lmed)
                    {
                        nvlmed.Add(med);
                    }
                    var nvmms = new MmsMessage
                    {
                        Id = lel[0],
                        Text = lel[1],
                        MediaPaths = nvlmed,
                        Sender = lel[3]
                    };
                    nvlistmms.Add(nvmms);
                }
                datas.listmms = nvlistmms;
            }
                if (Preferences.ContainsKey("listmsg"))
            {
                List<messages> nvlistmsg = new List<messages>();
                var stringlist = Preferences.Get("listmsg", "");
                var sepdom = " /sd ";
                var sepel = " /sl ";
                string[] lmsg = stringlist.Split(sepdom);
                foreach (var dom in lmsg)
                {
                    string[] lel = dom.Split(sepel);
                    var nvdom = new messages
                    {
                        msgid = int.Parse(lel[0]),
                        tid= int.Parse(lel[1]),
                        numero = lel[2],
                        name = lel[3],
                        date=lel[4],
                        msg = lel[5],
                        type=lel[6],
                    };
                    nvlistmsg.Add(nvdom);
                }
                datas.listmsg = nvlistmsg;
            }

        }


    }
}
