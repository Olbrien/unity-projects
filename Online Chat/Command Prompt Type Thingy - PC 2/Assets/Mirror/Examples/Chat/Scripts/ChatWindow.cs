//using Mono.CecilX.Cil;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.Chat
{
    public class ChatWindow : MonoBehaviour
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(ChatWindow));

        public InputField chatMessage;
        public Text chatHistory;
        public Scrollbar scrollbar;

        bool firstMessage = true;
        bool shouldReturn;
        bool joined;
        bool left;
        bool joinedExit;
        bool leftExit;

        string theText;
        string temporaryName;
        float time;
        float timer;
        int secondsToClear;
        bool clearText;

        public void Awake()
        {
            Player.OnMessage += OnPlayerMessage;
        }

        void Update()
        {
            if (joined)
            {
                timer += Time.deltaTime;

                if (timer > 0.5f)
                {
                    chatMessage.text = ".joined";
                    OnSend();                    

                    timer = 0;
                    joined = false;                    
                }
            }

            else if (left)
            {
                timer += Time.deltaTime;

                if (timer > 0.5f)
                {
                    chatMessage.text = ".left";
                    OnSend();

                    timer = 0;
                    left = false;
                }
            }

            if (joinedExit || leftExit)
            {
                timer += Time.deltaTime;

                if (timer > 0.5f)
                {
                    Player player = NetworkClient.connection.identity.GetComponent<Player>();
                    player.playerName = temporaryName;

                    timer = 0;
                    joinedExit = false;
                    leftExit = false;
                }
            }


            if (clearText)
            {
                time += Time.deltaTime;

                if (time > secondsToClear)
                {
                    chatHistory.text = "";
                    clearText = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnSend();
            }

            if (Input.GetKeyDown(KeyCode.End))
            {                
                chatMessage.text = ".j";

                OnSend();

                shouldReturn = true;
            }
        }

        void OnPlayerMessage(Player player, string message)
        {
            if (player.isLocalPlayer)
            {
                if (firstMessage)
                {
                    if (player.playerName == "" || player.playerName == null) { theText = $"<color=orange>{player.playerName}</color>{message}";  }
                    else { theText = $"<color=orange>{player.playerName}: </color>{message}";  }                    
                }

                else if (!firstMessage)
                {
                    if (player.playerName == "" || player.playerName == null) { theText = $"\n<color=orange>{player.playerName}</color>{message}"; }
                    else { theText = $"\n<color=orange>{player.playerName}: </color>{message}"; }
                    firstMessage = true;
                }
            }

            else if (!player.isLocalPlayer)
            {
                if (firstMessage)
                {
                    if (player.playerName == "" || player.playerName == null) { theText = $"\n<color=cyan>{player.playerName}</color>{message}"; }
                    else { theText = $"\n<color=cyan>{player.playerName}: </color>{message}"; }
                    firstMessage = false;
                }

                else
                {
                    if (player.playerName == "" || player.playerName == null) { theText = $"<color=cyan>{player.playerName}</color>{message}"; }
                    else { theText = $"<color=cyan>{player.playerName}: </color>{message}"; }                        
                }
            }

            string prettyMessage = theText;
            AppendMessage(prettyMessage);

            logger.Log(message);
        }

        public void OnSend()
        {
            GameMessages();
            if (shouldReturn) { shouldReturn = false; return; }

            if (chatMessage.text.Trim() == "")
                return;

            // get our player
            Player player = NetworkClient.connection.identity.GetComponent<Player>();

            // send a message
            player.CmdSend(chatMessage.text.Trim());

            chatMessage.text = "";
        }

        internal void AppendMessage(string message)
        {
            StartCoroutine(AppendAndScroll(message));
        }

        IEnumerator AppendAndScroll(string message)
        {
            chatHistory.text += message + "\n";

            // it takes 2 frames for the UI to update ?!?!
            yield return null;
            yield return null;

            // slam the scrollbar down
            scrollbar.value = 0;
        }



        ///////////////////////////////
        ///   Game Messages         ///
        ///////////////////////////////

        void GameMessages()
        {
            // Clear Messages

            if (chatMessage.text.Trim() == ".clear")
            {
                chatHistory.text = "";
                chatMessage.text = "";

                shouldReturn = true;
                return;
            }

            else if (chatMessage.text.StartsWith(".clear "))
            {
                string st = chatMessage.text.Remove(0, 7);

                if (st.Trim() == "")
                {
                    shouldReturn = true;
                    chatMessage.text = "";
                    return;
                }

                else if (int.TryParse(st, out int number))
                {
                    shouldReturn = true;
                    chatMessage.text = "";
                    secondsToClear = number;
                    time = 0;
                    clearText = true;
                    return;
                }

                else
                {
                    shouldReturn = true;
                    chatMessage.text = "";
                    return;
                }
            }

            // Change Name

            Player player = NetworkClient.connection.identity.GetComponent<Player>();

            if (chatMessage.text.StartsWith(".name "))
            {
                string st = chatMessage.text.Remove(0, 6);
                
                if (st.Trim() == "")
                {
                    player.playerName = "";
                    shouldReturn = true;
                    chatMessage.text = "";
                    return;
                }

                else
                {
                    player.playerName = st;
                    shouldReturn = true;
                    chatMessage.text = "";
                    return;
                }                
            }

            if (chatMessage.text.Trim() == ".name")
            {
                temporaryName = player.playerName;
                player.playerName = "";
                shouldReturn = true;
                chatMessage.text = "";
                return;
            }

            // Quit

            if (chatMessage.text.Trim() == ".quit")
            {
                Application.Quit();
            }

            // Join the Room            

            if (chatMessage.text.Trim() == ".j")
            {
                temporaryName = player.playerName;
                player.playerName = "";
                joined = true;
                shouldReturn = true;
                chatMessage.text = "";
                return;
            }

            if (chatMessage.text.Trim() == ".joined")
            {
                chatMessage.text = $"<color=yellow>{temporaryName}</color> joined the room.\n";
                joinedExit = true;

                return;
            }

            // Left the Room            

            if (chatMessage.text.Trim() == ".l")
            {
                temporaryName = player.playerName;
                player.playerName = "";
                left = true;
                shouldReturn = true;
                chatMessage.text = "";
                return;
            }

            if (chatMessage.text.Trim() == ".left")
            {
                chatMessage.text = $"<color=red>{temporaryName}</color> left the room.\n";
                leftExit = true;

                return;
            }

            // List of Commands

            if (chatMessage.text.Trim() == ".list" || chatMessage.text.Trim() == ".help")
            {
                string prettyMessage = $".clear / .clear seconds / .name (can be empty) / .quit / .j / .l / .list\n" +
                                       $"Mensagens do .j (join) e do .l (leave) só funcionam com o Host (neste caso telemóvel).\n" +
                                       $"Isto funciona melhor se o host for o telemóvel. Não te esquecas de tirar a firewall do PC.\n" +
                                       $"Têm que estar conectados à mesma rede para funcionar. Qualquer coisa, hotspot da Vodafone.\n" +
                                       $"Long Live Imperial UO\n";
                AppendMessage(prettyMessage);
                
                chatMessage.text = "";
                shouldReturn = true;
                return;
            }

        }
    }
}