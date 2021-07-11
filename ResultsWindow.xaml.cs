using ArtistArtDownloader.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArtistArtDownloader
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        private const int RESULT_IMAGES = 50;
        private const string LOADING_URI = "/Images/loading-buffering.gif";
        private const string LOADING_MSG = "Loading...";
        private const string NOTFOUND_URI = "/Images/not-found.jpg";
        private const string NOTFOUND_MSG = "No results found, please try again.";

        private GScraper.GoogleScraper _gScraper = null;

        // User agent info
        private readonly string _uaName, _uaVer, _uaURL, _uaEmail;

        public ResultsWindowViewModel ViewModel = null;
        public ResultsWindow()
        {
            InitializeComponent();

            ViewModel = this.DataContext as ResultsWindowViewModel;
            _gScraper = new GScraper.GoogleScraper();

            // Construct the user agent string
            _uaName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            _uaVer = System.Reflection.Assembly.GetEntryAssembly().ImageRuntimeVersion;
            _uaURL = "https://github.com/zrasco";
            _uaEmail = "31866081+zrasco@users.noreply.github.com";
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private async void Search()
        {
            // Set cursor to end of search text box and set focus
            TextBoxSearchTerm.CaretIndex = TextBoxSearchTerm.Text.Length;
            TextBoxSearchTerm.Focus();

            if (!String.IsNullOrEmpty(ViewModel.SearchTerm))
            {
                try
                {
                    // Notify the user
                    ShowLoading();

                    // Current RESULT_IMAGES == 50
                    // Search for RESULT_IMAGES images. This can be adjusted, but with the refining option, 50 seems a good sweet spot.
                    // Can maybe make this customizable later.
                    var results = await _gScraper.GetImagesAsync(ViewModel.SearchTerm, RESULT_IMAGES);

                    if (results.Count > 0)
                    {
                        // Remove the loading icon
                        ViewModel.ImageResults.Clear();
                        SetStatusBarText("Done loading, please select an image.");

                        // Add the search results
                        foreach (var result in results)
                        {
                            ViewModel.ImageResults.Add(new ImageResultData()
                            {
                                ImageURL = result.Link,
                                ThumbnailURL = result.ThumbnailLink,
                                Width = result.Width.GetValueOrDefault(0),
                                Height = result.Width.GetValueOrDefault(0)
                            });
                        }
                    }
                    else
                    {
                        // No results found. Sometimes GScraper returns 0 results as a false positive, but not sure why. Searching again with similar text usually fixes it.
                        ShowNotFound();
                    }

                    ImageScroller.ScrollToTop();

                }
                catch (Exception ex)
                {
                    SetStatusBarText($"Unable to search: {ex.Message}", true);
                }
            }
        }


        private string GetTestJson()
        {
            return "[{\"ImageURL\":\"https://www.rollingstone.com/wp-content/uploads/2020/11/ACDC-Photo-2-PC_-Josh-Cheuse.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSNuBDZaTPUmRtQfAHWRlbsC6E2yu3DWVknwJN_NaaOuAePkN9Fw0SMzZ_-eky37_AH-M8&usqp=CAU\"},{\"ImageURL\":\"https://i.guim.co.uk/img/media/663df4ca465081b796ad837755286ce87c8eaaa7/475_93_6066_3640/master/6066.jpg?width=1200&height=1200&quality=85&auto=format&fit=crop&s=10820714d47104bd0ab70450903ed8eb\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSYGf_n5CGDO5P1vC03wZ3czwDta9HDPFvV3cOydozqcLVkvZzX-jDtOhFPj7Yw6baKhAA&usqp=CAU\"},{\"ImageURL\":\"https://www.gannett-cdn.com/presto/2020/11/12/USAT/09bd5c1c-d0b1-49eb-b940-bb54bf7f7e33-ACDC_Photo_1_PC__Josh_Cheuse.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR_NMpHoUvlEsxx_S1iyOaDkaXg5rloh73UTtvAIMgi6pS82u5QT0JuQDtGkbAV-zs2ixU&usqp=CAU\"},{\"ImageURL\":\"https://www.gannett-cdn.com/-mm-/333c1271c2393e4d5375c5f38ce5fedd5ad275a2/c=0-122-2400-1478/local/-/media/2015/09/09/DetroitFreePress/DetroitFreePress/635773773364614891-acdc.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSwwKFoAWmzLzSeg7w19i85-VmKHz0G4ehmhQZBVNUDJwkXIUWmMbWEemS9KQBbDwGPcvQ&usqp=CAU\"},{\"ImageURL\":\"https://i.ytimg.com/vi/pX93gmyya7c/maxresdefault.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT-vYUPcdpZCjn8imv7WSHg3tcNzR8A7gE-fXHf1aMg6jP5BmsjPNtqIDjxtLNYY7L-tSk&usqp=CAU\"},{\"ImageURL\":\"https://www.rollingstone.com/wp-content/uploads/2018/06/gettyimages-76838137-b37ffc8a-8fcb-4141-801d-f6a63a0dbeed.jpg?resize=1800,1200&w=450\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSDOTPFk4rvRRactD35il8ixsqOYjxEt7lxoCnrQlSl_YZTY9R5mDv0PIn_PVGeBo1r40M&usqp=CAU\"},{\"ImageURL\":\"https://townsquare.media/site/295/files/2020/10/ACDC-shot-in-the-dark.jpg?w=1200&h=0&zc=1&s=0&a=t&q=89\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTNbrE2ggb0nDChQR2Ddzlu_0WY0D1rv7-mrI3rB8aHXkhI5DHQdaTY7Rk9tcHGZQ6Ejkk&usqp=CAU\"},{\"ImageURL\":\"https://storage.googleapis.com/stateless-wknc-org/sites/1/2020/11/ACDC-Power-Up.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR2v51qgufZzr4yPgfR2oXe8WE-SlzhuJbkm1A2HRj8jJUSMplnXCatsZ_ifzR8ZAywLgo&usqp=CAU\"},{\"ImageURL\":\"https://media.newyorker.com/photos/59095f7eebe912338a374b3a/master/pass/Michaud-ACDC.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTx51AHQDctksPCsVTcYp-Y8FNbWa6qtI1VsknJJo46P42RXyVRYZqMCBPiZCtEdzEpJYA&usqp=CAU\"},{\"ImageURL\":\"https://static.independent.co.uk/s3fs-public/thumbnails/image/2020/07/23/13/acdc-0.jpg?width=1200\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRp59oZii3yQzzlbET1JLNOsB4MKb0NagOdqzrkG03VyHd6W3_hnnxPPDWGucyu-CxX9Kg&usqp=CAU\"},{\"ImageURL\":\"https://pyxis.nymag.com/v1/imgs/80e/669/f2c90acbb340a3e306b70864be3c296874-15-acdc.rsquare.w700.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQM8vpw3XU5IyiMHB6p3bfrZuEz5fGIIgJ5we9JVDSGR5l65q0ov_U7psfCGVnwGeaE9AA&usqp=CAU\"},{\"ImageURL\":\"https://lookaside.fbsbx.com/lookaside/crawler/media/?media_id=10159002062107930\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTL9hYZaGtroRD4viJvrkrrsBdrb-vcMatZsKkSyzXtTEnEWYvI-D9FkYgomK-fjvyefsA&usqp=CAU\"},{\"ImageURL\":\"https://www.gannett-cdn.com/-mm-/e4c82b80ab4cf67937b779105d29cf204b8f13c5/c=0-645-8269-5317/local/-/media/USATODAY/USATODAY/2014/11/28/635527720886356098-XXX-ACDC-05-014.jpg?width=660&height=373&fit=crop&format=pjpg&auto=webp\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRq_6e4brOGPSnz-gG8jcqTyvuz3a_RzZSqGex-6w7OYmjBqrCY3fW4ygDVvcvBGloh_4Y&usqp=CAU\"},{\"ImageURL\":\"https://static.stereogum.com/uploads/2020/11/ACDC-1605114470.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQq_Se5ojwqQIgyf45rusG1eKuC9q53sAtqpnkgihQbg4iWbdi8-uDCZdG6RmamEHBx6GU&usqp=CAU\"},{\"ImageURL\":\"https://pbs.twimg.com/profile_images/1310549160286982145/SgKZspfm_400x400.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQL__tpksOk8mtYI4hXY8NpkJpH5ICS2U53zb6BT1KJR9B7-NqotzvL1WNiTZbdyxvsv1o&usqp=CAU\"},{\"ImageURL\":\"https://www.rollingstone.com/wp-content/uploads/2021/01/angus-young-q-and-a.jpg?resize=1800,1200&w=450\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQQjDkR6KETd64Ap1GV-TqzjPJeJF8G5xmAxP1GYnVeVQpZlKVBuGacQqxOtUEmicLYwsw&usqp=CAU\"},{\"ImageURL\":\"https://blog.siriusxm.com/wp-content/uploads/2017/05/acdc-blog-crop.jpg?w=800\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVum_eBOqc4SAORzbB6jB83a1es71ESvzh30GZdIgbCNgVu5UbaHJ2phvjqYSwS-nfvsA&usqp=CAU\"},{\"ImageURL\":\"https://www.wearethepit.com/wp-content/uploads/2020/09/ACDC-leak-shot-4.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRpVE6OLm2v6looPqJRE_BiXhv0gua2t7OZ-7QoFusG5yLVHiXhSmDzXJdX24cURkjmp7U&usqp=CAU\"},{\"ImageURL\":\"https://consequence.net/wp-content/uploads/2020/09/ACDC.jpg?quality=80&w=1031&h=580&crop=1&resize=1031%2C580&strip\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSH8TxC4-ZTpCqpuRQTn3_THRikYS7sQY0CJZOxVIMXMuVb62-h2Ya8UwrOwTAXTJLR_sI&usqp=CAU\"},{\"ImageURL\":\"https://a57.foxnews.com/static.foxnews.com/foxnews.com/content/uploads/2018/11/931/524/ACDC-1970-Getty.jpg?ve=1&tl=1\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRNtL9HLTWxQB7i09t0YdcFH2vL_JMC0bPjebQ-k0gTuipAmt4PsXQX386kyeJI2Wsl22I&usqp=CAU\"},{\"ImageURL\":\"https://pyxis.nymag.com/v1/imgs/7d6/a8a/49251b8f320fa1f745790cc099b00c1dd7-acdc-.rsquare.w1200.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQTbilBDcLzEtkFkrdZ0MrS8bwYNhWvYDDQYA&usqp=CAU\"},{\"ImageURL\":\"https://i.guim.co.uk/img/media/abe4dd767c5584c1c5d49f120ce893b3e3c925ca/0_0_1192_715/master/1192.jpg?width=1200&quality=85&auto=format&fit=max&s=6c89f81c2faea14e64842a3e323740e2\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQQLjEsPae-NQU3DFKaAm_ZOWEuq26s7IewqQ&usqp=CAU\"},{\"ImageURL\":\"https://s3.amazonaws.com/media.thecrimson.com/photos/2020/11/03/010936_1346645.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQJ-upsdkSUE648_XOtF-6pbweIAvXsmiOs3Q&usqp=CAU\"},{\"ImageURL\":\"https://ichef.bbci.co.uk/news/976/cpsprodpb/8341/production/_98810633_a0c4f967-86b2-4b24-ac4d-e9d136cc6307.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT25l9W6Q-7IQXFwhirdsjHwsc6yEtXXIq91g&usqp=CAU\"},{\"ImageURL\":\"https://i.ytimg.com/vi/ga5qfM2-kog/maxresdefault.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTU6xUcD0287rgYcO8hOMo8AXjZkIRnaMSPWQ&usqp=CAU\"},{\"ImageURL\":\"https://www.nme.com/wp-content/uploads/2020/11/acdc-angusyoung-2000x1270-1.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSv-rDR0bLSrUXbCDbnvs2uSKAaegZQbYVtWw&usqp=CAU\"},{\"ImageURL\":\"https://static.onecms.io/wp-content/uploads/sites/20/2020/09/30/acdc.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRkMWiSDFiYXwcRTF3OwLT8rIJXs8Sk_mFHcQ&usqp=CAU\"},{\"ImageURL\":\"https://media.pitchfork.com/photos/5c5c4f23a824116b23ec5994/4:3/w_524,h_393,c_limit/ACDC.png\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSjHGx3o5QmE6GVB7XoShq_aHRxGntE32rYbw&usqp=CAU\"},{\"ImageURL\":\"https://images-na.ssl-images-amazon.com/images/I/71s6glEqRyL._SL1500_.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSQQqVtr1g5HGWyTPlh-lQy5d0CGWS2LnzzgA&usqp=CAU\"},{\"ImageURL\":\"https://townsquare.media/site/295/files/2021/01/ACDC-Sony.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQnt2jxYgdvQI8malbQGRBdTnA3SFeoDgShKA&usqp=CAU\"},{\"ImageURL\":\"https://static.independent.co.uk/2020/11/12/09/newFile-4.jpg?width=982&height=726&auto=webp&quality=75\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQQvuPzAorfinB8a5GQKOZ7d08nmNv6LdXV4Q&usqp=CAU\"},{\"ImageURL\":\"https://www.epicrights.com/wp-content/uploads/ACDC-Logo-2020.png\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTaIjLiaAEialkGE8jim10ucRGnYHZtCijJjg&usqp=CAU\"},{\"ImageURL\":\"https://cdn.mos.cms.futurecdn.net/uFYMDL3fkWEnVHzxgkLD6h.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcREHzlweoW7VFkSAKxMFBIIGHA7-62M2lDvuQ&usqp=CAU\"},{\"ImageURL\":\"https://www.revolvermag.com/sites/default/files/styles/image_750_x_420/public/media/images/article/dave-evans-acdc.jpg?itok=9mejJdJc&timestamp=1600806283\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTsDTE4vzq9WIcI8rRuEeiHIVaN8W61k19aPA&usqp=CAU\"},{\"ImageURL\":\"https://static.wikia.nocookie.net/disney/images/1/1c/MI0003885396.jpg/revision/latest?cb=20180129201025\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQnQjMqqVSd8qEZ55mvVc6gG4PD-RbOXVpXWA&usqp=CAU\"},{\"ImageURL\":\"https://i.ytimg.com/vi/xNhn1KOqq8g/maxresdefault.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRH_zCik8I36i4Vom_0_wlaYfVrQGS1Oj6vbQ&usqp=CAU\"},{\"ImageURL\":\"https://pwrup.acdc.com/assets/images/bg.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSoLbmBxlU6NVrt3qRV0KHu8dpNsvMpGVozQg&usqp=CAU\"},{\"ImageURL\":\"https://www.nme.com/wp-content/uploads/2020/01/brian.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQRDI0EqrBJpxPWhNxGNNRWgCtJ6kyBt9SKSw&usqp=CAU\"},{\"ImageURL\":\"https://upload.wikimedia.org/wikipedia/commons/b/bc/ACDC_in_St._Paul%2C_November_2008.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQRhEpRvc9y-pIJFf2Wsrl4lYY5xW-BPAJluw&usqp=CAU\"},{\"ImageURL\":\"https://ichef.bbci.co.uk/news/640/cpsprodpb/D88D/production/_115373455_gettyimages-1202070993.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQoGymmu1D4UQlNM8VEqpWoGc1DA950V7xrvA&usqp=CAU\"},{\"ImageURL\":\"https://pwrup.acdc.com/assets/images/og-pwrup.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSI_zIjQJRzDEZZyoW2UklyOOD0HjX5O12leg&usqp=CAU\"},{\"ImageURL\":\"https://i.pinimg.com/474x/1c/8b/e5/1c8be54a36e10dff6a96725b48dc9f53.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRd4BE9q0sq_fe9zuyR2jYHndJetYk7AIt7_g&usqp=CAU\"},{\"ImageURL\":\"https://www.guitarnoise.com/images/features/acdc-1024x724-1160x665.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTSihFbLHkjGPY_qN4av9kFtnwYWgZkViRzaA&usqp=CAU\"},{\"ImageURL\":\"https://pyxis.nymag.com/v1/imgs/7d6/a8a/49251b8f320fa1f745790cc099b00c1dd7-acdc-.2x.rsocial.w600.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQMpjvcL_2c5jbBJgFqKyHRfZR_EtNZHowyTQ&usqp=CAU\"},{\"ImageURL\":\"https://upload.wikimedia.org/wikipedia/commons/9/9e/ACDC_In_Tacoma_2009.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT_-O1l10TH7NgQvgdirtsfmEjMrIGI5IecXQ&usqp=CAU\"},{\"ImageURL\":\"https://s.abcnews.com/images/Entertainment/WireAP_bdb90b289d634a26affb5299dc2974de_16x9_1600.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQkU5IYVgupctXRJZ9P4oiKRs5tFHrZTBckrQ&usqp=CAU\"},{\"ImageURL\":\"https://www.jambase.com/wp-content/uploads/2020/10/acdc-shot-in-the-dark-video.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRVOkPz6_EWU7GiZJthFa30zKjnioO6IGU7Gw&usqp=CAU\"},{\"ImageURL\":\"https://1yd4xt11c7is39w2ckdxdls5-wpengine.netdna-ssl.com/wp-content/uploads/2020/10/ACDC_main_750.jpeg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSD-dj8TtyWxbaeaeLwa3zZh8rrTLPh6gjVVg&usqp=CAU\"},{\"ImageURL\":\"https://variety.com/wp-content/uploads/2020/11/acdc.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSSMtWRne_YQX7TJeDriGaWfLSAHafy-mbH7A&usqp=CAU\"},{\"ImageURL\":\"https://townsquare.media/site/366/files/2020/09/ac-dc-angus-young-playing-guitar-with-his-foot-in-the-air.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSazusohLw-34sA3WNBtjbfhuxSfa-jaDhK0g&usqp=CAU\"},{\"ImageURL\":\"https://static.billboard.com/files/media/acdc-back-in-black-album-cover-650-compressed.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTP4LmxgWMav0V3PmOpilZ03ypItV8-Gsc0AQ&usqp=CAU\"},{\"ImageURL\":\"https://assets.blabbermouth.net/media/acdcvideo2020_638.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRaZeahcdn-kB9y5hiB-UnsCv0T_HJLRkKMZg&usqp=CAU\"},{\"ImageURL\":\"https://www.musicweek.com/cimages/8908a9171f836d356f327d02f6f292e1.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRusUTrOR2kSY3ehQynUWFqdxBbkwkfWBqAeg&usqp=CAU\"},{\"ImageURL\":\"https://metalinjection.net/wp-content/uploads/2020/11/acdc-2020.png\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQAEqUCAMVJlud0I8rd5HEfRJ3MZyz8-90duA&usqp=CAU\"},{\"ImageURL\":\"https://lookaside.fbsbx.com/lookaside/crawler/media/?media_id=263012381947161\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRbgls2TyEj5R37IWOPsTd0WYafaMg4mCo3vA&usqp=CAU\"},{\"ImageURL\":\"https://static.billboard.com/files/2020/11/ACDC-Rock-or-Bust-2016-Billboard-1548-1605285015-compressed.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTaCaxOAklEddCRxbXZ6-wlD6YBCTuQA3Ywbw&usqp=CAU\"},{\"ImageURL\":\"https://k-zap.org/wp-content/uploads/2019/04/acdc.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT3mSoTpcRy5t1fpBBYVuCZPKDKykA6uK_D2A&usqp=CAU\"},{\"ImageURL\":\"https://media-cldnry.s-nbcnews.com/image/upload/newscms/2017_46/2231641/171118-malcolm-young-3-1007a-rs.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT_IZeeE72dF4mt7IRQmEkMN8RlfTqnF0fuRA&usqp=CAU\"},{\"ImageURL\":\"https://www.rockcellarmagazine.com/wp-content/uploads/2020/10/acdc-demon-fire.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRgAiNoPAkr9g2d6Xeyvx2-eUHC7LUncIHv1A&usqp=CAU\"},{\"ImageURL\":\"https://pwrup.acdc.com/assets/images/rotatingboxFrontOff.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRFkEVP3zW97SyGls6pA1HV6nENhDye-hPdYA&usqp=CAU\"},{\"ImageURL\":\"https://snworksceo.imgix.net/dpn-34s/63c8edf2-c1a6-4f40-86c9-4ae3ff204992.sized-1000x1000.png?w=1000\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcShSVo8DHSceXziEXMWawIJVOhKnLGrw-qweg&usqp=CAU\"},{\"ImageURL\":\"https://media.newyorker.com/photos/5909511dc14b3c606c1036b8/master/pass/EPA-h_01664504-580.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRqFztaeavYLljTHWRoB670RPlB79Rc5Nr0tw&usqp=CAU\"},{\"ImageURL\":\"https://i0.wp.com/liveforlivemusic.com/wp-content/uploads/2020/10/acdc-shot-in-the-dark-1.png?fit=2880%2C1440&ssl=1\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS3TNAPSpMhOfJDIbKBh37zQsbRa6kbylyLQA&usqp=CAU\"},{\"ImageURL\":\"https://consequence.net/wp-content/uploads/2020/11/ACDC-Ranked.jpg?quality=80\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTVUltMQ90nt_PWliPZSbC4NgprM8pwBL4-Yg&usqp=CAU\"},{\"ImageURL\":\"https://www.hubcityspokes.com/sites/default/files/field/image/ACDC.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRhyvbng3Cm7cFvtbsr7okvHdSxz_PAMTtEog&usqp=CAU\"},{\"ImageURL\":\"https://rockcelebrities.net/wp-content/uploads/2021/06/acdc-richard-ramirez.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSoQfXdtBigf6KSjGz-p02GWRyiqpPHpWpR3Q&usqp=CAU\"},{\"ImageURL\":\"https://www.premierguitar.com/media-library/eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpbWFnZSI6Imh0dHBzOi8vYXNzZXRzLnJibC5tcy8yNjYwMjA0NS9vcmlnaW4uanBnIiwiZXhwaXJlc19hdCI6MTYzMDc4MzgwNX0.EJDtGB2QyhUfBPQMTxXAmYr25soLLcUVa5Gh-1KU7ec/image.jpg?width=1200&height=1565\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQedgwqk-w9iJ80jks6JSpB3vMhGqpvXTvNiQ&usqp=CAU\"},{\"ImageURL\":\"https://floridatheatre.com/assets/Banner-CAL-ACDC.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR4g54j8heChtoYszH-nLxir4kHsnHs6LTF0g&usqp=CAU\"},{\"ImageURL\":\"https://www.loudandquiet.com/files/2017/01/ac-dc.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS9GviFn9w_wIamIOXsGBNKxw9485MOPvARXg&usqp=CAU\"},{\"ImageURL\":\"https://abelreels.com/media/catalog/product/cache/3/thumbnail/760x/17f82f742ffe127f42dca9de82fb58b1/a/c/acdc-front.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTN3kZlxvzb8fYszy3fkR8C7ryYkiBzTobg6g&usqp=CAU\"},{\"ImageURL\":\"https://i.pinimg.com/originals/90/8f/38/908f38421a9ac5c7b24652ccc9177c6b.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTloffiDtzG8VJZ6fZXCKQ_mNwU68VEJo_0Cw&usqp=CAU\"},{\"ImageURL\":\"https://www.straight.com/files/v3/styles/gs_standard/public/images/20/02/acdclogo.jpg?itok=PUman4iW\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRQUKTIVPJ21LsFFCs6XZXTbPB1_HYb7Zwp5g&usqp=CAU\"},{\"ImageURL\":\"https://images.theconversation.com/files/46442/original/rnpy4pf5-1397540595.jpg?ixlib=rb-1.1.0&q=45&auto=format&w=1200&h=1200.0&fit=crop\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQZ3mMy0UHml4kgjV82m3cSdkJ3P7QhDYf63Q&usqp=CAU\"},{\"ImageURL\":\"https://1000logos.net/wp-content/uploads/2016/10/ACDC-log%D0%BE.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcROpkTxOmmF-Wav3WFgABTiqnVaLm4jFW75Zw&usqp=CAU\"},{\"ImageURL\":\"https://www.wearethepit.com/wp-content/uploads/2021/01/ACDC-Realize-Video-678x381.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQi7yt_ZdxL54sGLotSv5RWGwhPK7do_LC0WQ&usqp=CAU\"},{\"ImageURL\":\"https://cdns-images.dzcdn.net/images/artist/ad61d6e0fa724d880db979c9ac8cc5e3/500x500.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT0WLQIcj1nLW-mJ_W99X6D-BSUf4e8Zxg1fQ&usqp=CAU\"},{\"ImageURL\":\"https://cdn.images.express.co.uk/img/dynamic/35/590x/acdc-new-album-1340970.jpg?r=1601304363360\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQMursX8U12oYZYbodM9DeKbdwuVkZTLk5S5A&usqp=CAU\"},{\"ImageURL\":\"https://townsquare.media/site/366/files/2020/09/ac-dc-pwr-up-ad.jpg?w=1080&q=75\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRFIBK7ZGMCkF8m5oTLgDKAPiTwlcnfFaLH-g&usqp=CAU\"},{\"ImageURL\":\"https://static.standard.co.uk/s3fs-public/thumbnails/image/2020/09/21/12/angus-young-acdc-2109.jpg?width=968&auto=webp&quality=75&crop=968%3A645%2Csmart\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTJtKxPTIY-AGywZiXjo4zzLg6UvsBg13-H1A&usqp=CAU\"},{\"ImageURL\":\"https://www.legacyfoodhall.com/wordpress/uploads/sites/2/2021/04/acdc.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSqhIpGKZn45JYsUbfV1GZC38AGSZ32UDRK9g&usqp=CAU\"},{\"ImageURL\":\"https://files.thehandbook.com/uploads/2014/10/acdc-retirement-rumors.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSPMzeb9dQBRbc9xkWM4PRARzIOoYzbTFfuEQ&usqp=CAU\"},{\"ImageURL\":\"https://lite-images-i.scdn.co/image/ab67706f00000002eb6ecd02b6e08dfdefcdcc21\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcShTNSl8eyNsc_4oTPAPT5GxRyY6q7biKjmVQ&usqp=CAU\"},{\"ImageURL\":\"https://i.guim.co.uk/img/media/01ef9774a30becaa9959320784862d1b2faab52f/0_49_2992_1795/master/2992.jpg?width=1200&height=1200&quality=85&auto=format&fit=crop&s=e2efe5746e8b4cac33700ee3cf5a157b\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcScxqW88mv4enf3LZp-gkUidWD317gt9WVa7Q&usqp=CAU\"},{\"ImageURL\":\"https://pbs.twimg.com/media/EjuxlmNUwAATyC5.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRd5XPFxqQBDdFw54Qnezqb_r0_vhDbbmuntA&usqp=CAU\"},{\"ImageURL\":\"https://lookaside.fbsbx.com/lookaside/crawler/media/?media_id=10158682874152930\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQPViNd7z8YT2klfolivbzj625bUF9JGnJv3g&usqp=CAU\"},{\"ImageURL\":\"https://s.abcnews.com/images/Entertainment/ac-dc-gty-jt-171118_16x9_992.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQolkGW1tS9uoAG4hzvK1Wm6gkxkpPDfmJCjw&usqp=CAU\"},{\"ImageURL\":\"https://musicglue-images-prod.global.ssl.fastly.net/ac-dc/profile/images/ACDC_Logo.png?u=aHR0cHM6Ly9kMTgwcWJkYTZvN2U0ay5jbG91ZGZyb250Lm5ldC9lNy81MC8wMC84Yy9lZS8zMC80Ni9hMS85MC9kZS85MC84MS8wNy84My8xNi84Ni9BQ0RDX0xvZ28ucG5n&mode=contain&width=300&v=2\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRMFCLcaPke9yOTeJkJvB6r5_I3CLHLwqGdaw&usqp=CAU\"},{\"ImageURL\":\"https://images.backstreetmerch.com/images/products/bands/posters/acdc/bsi_acdc43.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSvHUQXsp7j7MpUoG18wYKtBepUxYBf1wBOpw&usqp=CAU\"},{\"ImageURL\":\"https://data.mothership.tools/mothershiptools-ukprod/wp-content/uploads/2017/10/acdc-rectangle-1024x681.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRkTQy3FVoJ0i6ZMqeetoLSmVbZ3t7treaC2Q&usqp=CAU\"},{\"ImageURL\":\"https://m.media-amazon.com/images/I/61uycmADL-L._AC_.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSiwGwEFbJlHw33_PeWAviXyd78p88HultW2g&usqp=CAU\"},{\"ImageURL\":\"https://res.cloudinary.com/dhh19fozh/q_auto:good,f_auto,dpr_1.0/w_auto:breakpoints_85_850_10_10:382/jb7production-uploads/2018/03/angus-young-russo-crop-3-acdc-1200x658.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSaIE0A2Wi7p_z4iKGpKOrtuWt3trTK96B6Zg&usqp=CAU\"},{\"ImageURL\":\"https://i0.wp.com/liveforlivemusic.com/wp-content/uploads/2021/01/ACDC-Realize-Music-Video.png?fit=740%2C390&ssl=1\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSaosmTBuiikO_REjkT6LlaRX7akn_0EClACg&usqp=CAU\"},{\"ImageURL\":\"https://ca-times.brightspotcdn.com/dims4/default/080b3c4/2147483647/strip/true/crop/2048x1475+0+0/resize/840x605!/quality/90/?url=https%3A%2F%2Fcalifornia-times-brightspot.s3.amazonaws.com%2F80%2Fe6%2F3ab85a630c462b670341f2c9693c%2Fla-et-ms-ac-dc-dodger-stadium-review-20150929-001\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ-noyZk-c-dDs-zXlHLfUN2JiGM8fI9k8gGg&usqp=CAU\"},{\"ImageURL\":\"https://www.raymond-weil.com/wp-content/uploads/2018/04/ACDC2.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTeiQn94ZYHKzTM_LmxhUQ0DRrm7tDtcMLFCA&usqp=CAU\"},{\"ImageURL\":\"https://static.wikia.nocookie.net/marvelcinematicuniverse/images/8/8e/ACDC.jpg/revision/latest?cb=20170103020752\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTenYjOYKktDRM0aG21lte1FeQ6UmkClbWagA&usqp=CAU\"},{\"ImageURL\":\"https://i.ytimg.com/vi/v2AC41dglnM/mqdefault.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR5lNsW1WdAoU0BsTRJknoNfNA2PuIfCQVCSw&usqp=CAU\"},{\"ImageURL\":\"https://cdn.theculturetrip.com/wp-content/uploads/2017/11/angus-young-acdc-guitarist--ed-villflickr-.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRt4UW9fNSN-leDe8bgI7N2FVlYSRAb4ABPCw&usqp=CAU\"},{\"ImageURL\":\"https://static.billboard.com/files/media/acdc-billboard-650-compressed.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTxZMncpr7y59IIJ4gTQEUy0mP11MmBZFNs-A&usqp=CAU\"},{\"ImageURL\":\"https://www.legacyfoodhall.com/wordpress/uploads/sites/2/2021/04/acdc-again.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRVzTkKo0OEA8J7y8gwoth9k9ywvWL6JZrRpg&usqp=CAU\"},{\"ImageURL\":\"https://www.inthestudio.net/wp-content/uploads/2020/06/acdc-83detroit-495x400.jpg\",\"ThumbnailURL\":\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSjLy5BKePSOIF7DY-iawox4s9-TFjT8igeXQ&usqp=CAU\"}]";
        }

        private void ButtonImage_Click(object sender, RoutedEventArgs e)
        {
            // We use a trick here to get the binded item in the button tag
            // The tag has to be set in XAML to {Binding} with no parameter for this to work
            var imageData = ((sender as Button)?.Tag as ImageResultData);
            string imageURL = imageData?.ImageURL;

            if (imageURL == LOADING_MSG || imageURL == NOTFOUND_MSG)
                // Do nothing if user clicks loading/not found image
                return;

            // Save the image to the correct path
            try
            {
                string savePath = ViewModel.ArtistEntries.FirstOrDefault()?.FullPath + @"\folder.jpg";

                try
                {
                    // Try to save the image URL first. This doesn't always work and there are a lot of edge cases to account for.
                    // So rather than account for all of them, use a nested exception handler with the thumbnail as a fallback image.
                    SaveImage(imageURL, savePath, ImageFormat.Jpeg);
                }
                catch (Exception)
                {
                    // Try to get the thumbnail as a fallback... this should always work (hopefully)
                    // If this still fails we'll definitely want to abort. The error should get caught in the outer exception handler.

                    SaveImage(imageData.ThumbnailURL, savePath, ImageFormat.Jpeg);
                    SetStatusBarText("Warning: Primary image download failed, but was able to get thumbnail instead.");
                }
                
                // Clear the images and show loading icon
                ShowLoading($"Saved to {savePath}!");

                // Go on to next item
                ViewModel.ArtistEntries.RemoveAt(0);

                if (ViewModel.ArtistEntries.Count() > 0)
                {
                    // Begin next search
                    ViewModel.ResultsHeader = $"Refine results for artist: {ViewModel.ArtistEntries.FirstOrDefault()?.Name}";
                    ViewModel.SearchTerm = ViewModel.ArtistEntries.FirstOrDefault()?.Name;
                    Search();
                }
                else
                {
                    // All done
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                SetStatusBarText("Error: " + ex.Message, true);
            }

        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set first item
            ViewModel.ResultsHeader = $"Refine results for artist: {ViewModel.ArtistEntries.FirstOrDefault()?.Name}";
            ViewModel.SearchTerm = ViewModel.ArtistEntries.FirstOrDefault()?.Name;

            // Perform first search immediately
            Search();

        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            // Get the button image data via the tag {Binding} trick
            var imageData = ((sender as Button).Tag as ImageResultData);
            string widthHeight = null;

            // Put the image URL in the status bar
            StatusBarControl.Foreground = System.Windows.Media.Brushes.Black;

            // Add the width and height if they exist
            if (imageData.Height > 0 && imageData.Width > 0)
                widthHeight = $" ({imageData.Width}x{imageData.Height})";

            SetStatusBarText(imageData.ImageURL + widthHeight);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Cleanup
            _gScraper.Dispose();
        }

        private void ShowLoading(string savedMsg = null)
        {
            ViewModel.ImageResults.Clear();
            ViewModel.ImageResults.Add(new ImageResultData() { ThumbnailURL = LOADING_URI, ImageURL = LOADING_MSG });
            SetStatusBarText(!string.IsNullOrEmpty(savedMsg) ? savedMsg + " "  + LOADING_MSG : LOADING_MSG);
        }

        private void ShowNotFound(string savedMsg = null)
        {
            ViewModel.ImageResults.Clear();
            ViewModel.ImageResults.Add(new ImageResultData() { ThumbnailURL = NOTFOUND_URI, ImageURL = NOTFOUND_MSG });
            SetStatusBarText(!string.IsNullOrEmpty(savedMsg) ? savedMsg + " " + NOTFOUND_MSG : NOTFOUND_MSG);
        }

        // Thank you user Charlie!
        // Source: https://stackoverflow.com/questions/24797485/how-to-download-image-from-url
        public void SaveImage(string imageUrl, string filename, ImageFormat format)
        {
            WebClient client = new WebClient();

            client.Headers.Add("User-Agent", $"User-Agent: {_uaName}/{_uaVer} ({_uaURL}; {_uaEmail}) generic-library/0.0");
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                bitmap.Save(filename, format);
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }

        private void SetStatusBarText(string message, bool isError = false)
        {
            if (isError)
                StatusBarControl.Foreground = System.Windows.Media.Brushes.Red;
            else
                StatusBarControl.Foreground = System.Windows.Media.Brushes.Black;

            ViewModel.StatusBarText = message;
        }
    }
}
