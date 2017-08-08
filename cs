using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int dealer = 0; int me = 0; int stage = -1; Deck myDeck = new Deck(); string myString; int funds = 1000; int bet = 0; int bet_input = 0;

        int maxfordealer = 16; //dealer cant draw on 16 or up

        public static void KnuthShuffle<T>(T[] array)
        {
            System.Random random = new System.Random();
            for (int i = 0; i < array.Length; i++)
            {
                int j = random.Next(i, array.Length); // Don't select from the entire array on subsequent loops
                T temp = array[i]; array[i] = array[j]; array[j] = temp;
            }
        }

        class Deck
        {
            public int numDecks = 0;
            public int[] cardValue;
            public int index;

            public void makeDeck(int num)
            {
                this.numDecks = num;                  //the benefit of this class is that it can contain multiple decks
                this.index = 0;                             //this is why the arrays are complicated
                this.cardValue = new int[52 * num];         //this class was quickly replaced for a more advanced version
                int l = 0;
                int k = 0;
                int j = 0;
                int m = 0;
                int n = 0;
                for(m=0;m<num;m++)
                {
                    n = m * 52;
                    for (l=1;l<11;l++)
                    {
                        j = 4 * l;
                        for (k = j-4+n; k < j+n; k++)
                        {
                            this.cardValue[k] = l;
                        }
                    }
                    for(l=10;l<14;l++)
                    {
                        j = 4 * l;
                        for (k = j - 4+n; k < j+n; k++)
                        {
                            this.cardValue[k] = 10;
                        }
                    }
                }
                KnuthShuffle<int>(this.cardValue);

            }

            public int drawFromDeck()
            {
                int i = 0;
                if(this.index<this.cardValue.Length)
                {
                     i = this.cardValue[index];
                    this.index++;
                }
                return i;
            }
         }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = new MediaElement();
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync("Hello, welcome to Black Jack!");
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        private void draw(object sender, RoutedEventArgs e)
        {
            if (stage != -1)
            {
                if (stage == 0)
                {
                    dealer = dealer + myDeck.drawFromDeck();
                    myString = dealer.ToString();
                    D.Text = myString;
                    me = me + myDeck.drawFromDeck();
                    myString = me.ToString();
                    M.Text = myString;
                }

                stage++;
                me = me + myDeck.drawFromDeck();
                myString = me.ToString();
                M.Text = myString;

                if (me > 21)
                {
                    endMatch();
                }
            }
        }

        private void pass(object sender, RoutedEventArgs e)
        {
            if (me < 22)
            {
                if (stage != -1)
                {
                    while ((me>=dealer)&&(dealer < maxfordealer)) 
                    {
                        dealer = dealer + myDeck.drawFromDeck();
                        myString = dealer.ToString();
                        D.Text = myString;
                    }
                    endMatch();
                }
                
            }
            else
            {
                
                endMatch();
            }
        }

        private void newB(object sender, RoutedEventArgs e)
        {
            bet = bet_input;
            if ((bet == 0)||(bet>funds))
            {
                Debug.Text = "Place your bet!";
            }
            else
            {
                DPlayer.Text = "Dealer";
                MPlayer.Text = "Player";

                funds = funds - bet;
                showFunds.Text = funds.ToString();
                M.Text = "0"; D.Text = "0";
                dealer = 0; me = 0; stage = 0;
                myDeck.makeDeck(3);
                Debug.Text = "Draw until you have close but not above 21, then pass.";
                foreach (int i in myDeck.cardValue)
                {
                   System.Diagnostics.Debug.Write(i.ToString());
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

           Int32.TryParse(BetAmount.Text, out bet_input);
       }

        public void endMatch()
        {
            stage = -1;
            Debug.Text = "Make a new bet and play again";
            if ((me<dealer)&& (dealer < 22) || (me > 21))
            {
                myString = "Winner";
                DPlayer.Text = "Dealer is the " + myString;
                myString = "Loser!";
                MPlayer.Text = "Player is the " + myString;
                if(funds==0)
                {
                    Debug.Text = "Sorry, You're broke!";
                }
               
            }
            else if((dealer<me) && (me < 22)|| (dealer > 21))
            {
                myString = "Winner!";
                MPlayer.Text = myString;
                myString = "Loser";
                DPlayer.Text = myString;
                funds = funds + 2 * bet;
                showFunds.Text = funds.ToString();
                if (funds >= 1000000)
                {
                    Debug.Text = "Thats it, You've played enough! Go rob someone else's casino!";
                }
            }
            else
            {
                myString = "Even";
                DPlayer.Text = myString;
                myString = "Even";
                MPlayer.Text = myString;
                funds = funds + bet;
                showFunds.Text = funds.ToString();
            }
        }
    }
}
