using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using System.ComponentModel;

namespace Yılan_Oyunu
{
    public enum Yon // Yilanın Yönleri
    {
        Yukarı,
        Asagı,
        Sag,
        Sol
    }

    public partial class Form1 : Form
    {

        // Gerekli Tanımlamalar
        public string isim = "";
        public float dakika1 = 0, saniye1 = 0;
        public string dakika_deger1 = "";
        public string saniye_deger1 = "";
        public int dakika2 = 0, saniye2 = 0;
        public float skor = 0;
        public int Kisi_Kaydedildi = 0;
        public int X = 1, Y = 1;
        public Yon Yn = Yon.Sag;
        public Lokasyon Yem = new Lokasyon(-1, -1);
        public int hiz_Secildi = 0;
        public int oyun_basladi = 0;
        public List<Lokasyon> kuyruk = new List<Lokasyon>();
        public int Oyun_bitti = 1;
        public int D_kontrol = 0;
        public int B_kontrol = 0;
        public int İlk_Yem = 0;
        public string oyunSure;
        public int zaman_kontrol = 0;
        public Thread OyunThread;
        public float sonSure;
        public string zaman;
        public float guncelSure;
        public float araSure;

      

        public Form1()
        {
            InitializeComponent();
            kuyruk.Add(new Lokasyon(0, 0));
        }



        //Yılan Yem Gibi gerekli metaryelleri oluşturan fonksiyon
        public void Yilan_Olustur()
        {

            Random rdm = new Random();
            Bitmap Yilan = new Bitmap(600, 505);

            // kuyruk Carpma kontrolü
            if (kuyruk.Count != 1)
            {
                for (int i = 0; i < kuyruk.Count; i++)
                {
                    if (kuyruk[i].x == X && kuyruk[i].y == Y)
                    {
                        oyunSure = zaman;
                        DosyayaYaz();
                        Oyun_bitti = 0;
                        zaman_kontrol = 0;
                        Oyun_yeniden();
                    }
                }
            }

            if (X == Yem.x && Y == Yem.y) // Yem Alma kontrolü
            {

                if (İlk_Yem == 0)
                {
                    sonSure = saniye2 + (dakika2 * 60);
                    araSure = sonSure;
                }
                else
                {
                    guncelSure = saniye2 + (dakika2 * 60);
                    araSure = guncelSure - sonSure;
                    sonSure = guncelSure;
                }
                
                //label7.Text = araSure.ToString();
               
                if (Yem.x == 45 || Yem.x == 1 || Yem.y == 1 || Yem.y == 41) // Yem duvara yakın ise +10 puan
                {
                    skor = skor + 10;
                }
                if (araSure ==0)
                {
                    skor = skor + 100;
                }
                else if (araSure <= 100) //Yem yenme süresine göre puan Hesaplama
                {
                    skor = skor + (100 / araSure);
                }
               
                label4.Text = (skor.ToString("0.#"));
                kuyruk.Add(new Lokasyon(Yem.x, Yem.y));
                Yem = new Lokasyon(-1, -1);
                İlk_Yem = 1;
            }


            // Yilan oluştu
            if (X <= 0 || Y <= 0 || X == 46 || Y == 42) // Yılan Duvara Çarparsa Oyun Bitirme
            {
                oyunSure = (zaman);   
                DosyayaYaz();
                Oyun_bitti = 0;
                zaman_kontrol = 0;
                Oyun_yeniden();
                
            }
            else
            {
                for (int i = 10 * (X - 1); i < 10 * X; i++) // Yılanı Oluşturma
                {
                    for (int j = 10 * (Y - 1); j < Y * 10; j++)
                    {
                        Yilan.SetPixel(i, j, Color.White);
                    }
                }


                // Random Yem Oluşturma 
                if (Yem.x == -1)
                {
                    Yem = new Lokasyon(rdm.Next(1, 45), rdm.Next(1, 41));
                }
                for (int i = 10 * (Yem.x - 1); i < 10 * Yem.x; i++)
                {
                    for (int j = 10 * (Yem.y - 1); j < Yem.y * 10; j++)
                    {
                        Yilan.SetPixel(i, j, Color.Red);
                    }
                }


                kuyruk[0] = new Lokasyon(X, Y);

                if (kuyruk.Count != 1) //Yılanı Kuyruğa ekleme
                {
                    for (int a = 0; a < kuyruk.Count; a++)
                    {
                        for (int i = 10 * (kuyruk[a].x - 1); i < 10 * kuyruk[a].x; i++)
                        {
                            for (int j = 10 * (kuyruk[a].y - 1); j < kuyruk[a].y * 10; j++)
                            {
                                Yilan.SetPixel(i, j, Color.White);
                            }
                        }
                    }
                }


                for (int i = kuyruk.Count - 1; i > 0; i--) //kaydırma İşlemi
                {
                    kuyruk[i] = kuyruk[i - 1];
                }
            }
            Oyun.Image = Yilan; // Yılanı PictureBox Yerleştirme
        }


        //----------------------------------------------------------------------------------//

        private void Oyun_Baslat() // Oyunun Çalıştırma Kısmı
        {

            OyunThread = new Thread(new ThreadStart(new Action(() =>  //Yılanın Hareket Etmesi
            {
                zaman_kontrol = 1;

                while (Oyun_bitti == 1)
                {
                    oyun_basladi = 1;                                           
                    if (Yn == Yon.Sag) X++;
                    if (Yn == Yon.Sol) X--;
                    if (Yn == Yon.Yukarı) Y--;
                    if (Yn == Yon.Asagı) Y++;

                    //MessageBox.Show(X + "  " + Y);
                    Yilan_Olustur();
                    // Yılanın hızını Belirleme
                    if (radioButton1.Checked == true)
                    {
                        Thread.Sleep(140);
                    }
                    if (radioButton2.Checked == true)
                    {
                        Thread.Sleep(90);
                    }
                }
            })));
        }

        //------------------------------------------------------------------------------------------

        private void Oyun_yeniden() // Oyunu Yeniden Başlatma veya kapatma kısmı
        {
            DialogResult Dr = new DialogResult();
            Dr = MessageBox.Show("Yeni Oyun", "Kaybettiniz", MessageBoxButtons.YesNo);
            if (Dr == DialogResult.Yes)
            {
                dakika1 = 0; saniye1 = 0;
                dakika_deger1 = "";
                saniye_deger1 = "";
                dakika2 = 0; saniye2 = 0;
                İlk_Yem = 0;
                zaman_kontrol = 1;
                skor = 0;
                label4.Text = "0";
                X = 1;
                Y = 1;
                Yem = new Lokasyon(-1, -1);
                kuyruk = new List<Lokasyon>();
                kuyruk.Add(new Lokasyon(0, 0));
                Oyun_bitti = 1;
                D_kontrol = 0;
                Yn = Yon.Sag;
                B_kontrol = 1;
                timer1.Enabled = true;
                timer2.Enabled = true;
               
            }
            else
            {
                Application.Exit();
            }
        }


        private void DosyayaYaz() //İsim Süre Puan gibi Verileri dosyaya Kaydetme
        {

            StreamWriter SW = File.AppendText("Kayıtlar");
            SW.WriteLine(isim+ "    " +oyunSure + "    "+ (skor.ToString("0.#")));
            SW.Close();

        }
        readonly string DosyaYolu = "Kayıtlar";

        

        public class Lokasyon //Tekrar kullanılabilirlik sağlama
        {
            public int x, y;

            public Lokasyon(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

   
        //---------------------------------------------------------------------------------------------------
        private void button1_Click(object sender, EventArgs e) //Yardım Kısmı
        {
            MessageBox.Show("Mustafa Mert Kaya V.beta\n\n" +
                          "**********************Yılan Oyununa Hoş Geldiniz**********************\n" +
                          "Oyunumuz belirli bir alan içerisinde rasgele üretilen yemleri \n" +
                          "kuyruğumuza ekleyerek ve puanları toplayarak elimizden geldiğince\n" +
                          "ölmeden en yüksek puanı almaktır\n\n" +
                          "Oyunu oynamak için; önce isim yazılıp 'Kişiyi Kaydet' butonuna \n" +
                          "basılmalıdır,Sonrasında seviye seçilmelidir,'B' tuşu ile oyunu başlatılır\n" +
                          "'D' tuşu ile duraklatıp tekrar 'D' sürdürülebilirsiniz, yön tuşları\n" +
                          "ile yılanı yönlendirebilirsiniz.\n\n" +
                          "Puanlama: 100/'önceki yemden sonra geçen süre' olarak hesaplanır.\n" +
                          "Yem 100 saniye içinde yenmesse puan verilmeyecektir. köşe noktalardan\n" +
                          "yenen yemler için ise ekstra 10 puan alınacaktır.\n" +
                          "                                       ****İyi Oyunlar****");
        }


        private void button2_Click(object sender, EventArgs e)
        {
            isim = textBox1.Text;
            textBox1.Text = "";
            Kisi_Kaydedildi = 1;
            MessageBox.Show(isim + " Kaydedildi");
        }


        private void button3_Click(object sender, EventArgs e) // Dosyayı Açma
        {
            Process.Start(DosyaYolu);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e) //Form Kapandığında Oyunu Kapatma 
        {
            Application.Exit();
        }


        private void timer1_Tick(object sender, EventArgs e) // Oyun Geçen Süre Tutma
        {
            if (saniye1 < 10) saniye_deger1 = ("0" + Convert.ToString(saniye1) ); else saniye_deger1 = Convert.ToString(saniye1);
            if (dakika1 < 10) dakika_deger1 = ("0" + Convert.ToString(dakika1) ); else saniye_deger1 = Convert.ToString(dakika1);
           
            if (zaman_kontrol==1)
            {
                zaman = (dakika_deger1 + ":" + saniye_deger1);
                label5.Text = (zaman) ;
            }
            
            if ((saniye1 == 59))
            {
                saniye1 = 0;
                dakika1 = dakika1 + 1;
            }
            saniye1 = saniye1 + 1;
        }

        

        private void timer2_Tick(object sender, EventArgs e) // Yem yendiğinde puan hesabı için zamanlama 
        {

            //label7.Text = (Convert.ToString(dakika2) + " : " + Convert.ToString(saniye2));

            if ((saniye2 == 59))
            {
                saniye2 = 0;
                dakika2 = dakika2 + 1;
            }
            saniye2 = saniye2 + 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }


        //----------------------------------------------------------------------------------------------
        [Obsolete]
        private void Form1_KeyUp(object sender, KeyEventArgs e) // Tuşları Kullanmak için Gerekli İşlemler
        {
            if (B_kontrol == 0)
            {
                if (e.KeyCode == Keys.B) // B ile Oyun Başlama
                {

                    if ((radioButton1.Checked == true || radioButton2.Checked == true) && Kisi_Kaydedildi == 1 && isim != "" && oyun_basladi == 0)
                    {
                        panel1.Visible = false;
                        timer1.Enabled = true;
                        timer2.Enabled = true;
                        label4.Visible = true;
                        label5.Visible = true;
                        button1.Visible = false;
                        label6.Visible = false;
                        button3.Visible = false;
                        button2.Visible = false;
                        textBox1.Visible = false;
                        radioButton1.Visible = false;
                        radioButton2.Visible = false;
                        label7.Visible = true;
                        Oyun_Baslat();
                        if (OyunThread.ThreadState == System.Threading.ThreadState.Unstarted) OyunThread.Start(); // Gereksiz çalışmayı engelleme

                    }
                }
            }

                if (oyun_basladi == 1)
                {
                    if (e.KeyCode == Keys.D) // D ile Oyun Durdurma devam ettirme
                    {

                        if (D_kontrol % 2 == 0)
                        {
                            timer2.Enabled = false;
                            OyunThread.Suspend(); // thread durdurma
                            Oyun_bitti =0;
                            timer1.Enabled = false;
                            
                        }
                        if (D_kontrol % 2 == 1)
                        {
                        
                            Oyun_bitti = 1;
                            OyunThread.Resume(); // thread devam ettirme
                            timer1.Enabled = true;
                            timer2.Enabled = true;
                        }
                        D_kontrol = D_kontrol +1;
                    }
                }

                // Tuşları Kullanarak Yılanı Yönlendirme
                if (Yn != Yon.Asagı) // Yılanın Terse Gitmesini engelleme
                {
                    if (e.KeyCode == Keys.Up) // Yılanı Yönlendirme
                    {
                        Yn = Yon.Yukarı;
                    }
                }

                if (Yn != Yon.Yukarı)
                {
                    if (e.KeyCode == Keys.Down)
                    {
                        Yn = Yon.Asagı;
                    }
                }

                if (Yn != Yon.Sol)
                {
                    if (e.KeyCode == Keys.Right)
                    {
                        Yn = Yon.Sag;
                    }
                }

                if (Yn != Yon.Sag)
                {
                    if (e.KeyCode == Keys.Left)
                    {
                        Yn = Yon.Sol;
                    }
                }
            
        }
    }
}
