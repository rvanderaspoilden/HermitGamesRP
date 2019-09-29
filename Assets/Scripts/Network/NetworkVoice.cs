using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkVoice : MonoBehaviour
    {
        [SerializeField] private static int FREQUENCY = 8000;
        [SerializeField] private int lastSample = 0;
        [SerializeField] private bool notRecording;
        [SerializeField] private AudioClip sendingClip;
        [SerializeField] private bool sending;
        [SerializeField] private new AudioSource audio;

        private bool isTalking;

        private NetworkIdentity identity;

        private void Start() {
            this.audio = GetComponent<AudioSource>();
            this.identity = GetComponent<NetworkIdentity>();
            this.notRecording = true;
        }

        private void Update() {
            if (!this.identity.IsMine()) {
                return;
            }

            this.isTalking = Input.GetKey(KeyCode.X);
        }

        void FixedUpdate() {
            if (this.isTalking) {
                if (notRecording) {
                    notRecording = false;
                    sendingClip = Microphone.Start(null, true, 10, FREQUENCY);
                    sending = true;
                } else if (sending) {
                    int pos = Microphone.GetPosition(null);
                    int diff = pos - lastSample;

                    if (diff > 0) {
                        float[] samples = new float[diff * sendingClip.channels];
                        sendingClip.GetData(samples, lastSample);

                        byte[] ba = ToByteArray(samples);

                        VoicePacket packet = new VoicePacket(ba, sendingClip.channels, this.identity.GetNetworkID());

                        NetworkManager.instance.GetSocket().Emit("Packet::Voice", JSONObject.Create(JsonUtility.ToJson(packet)));

                    }
                    lastSample = pos;
                }
            } else {
                if (sending) {
                    this.sending = false;
                    Microphone.End(null);
                    notRecording = true;
                    sendingClip = null;
                    lastSample = 0;
                }
            }

        }

        public void PlayVoiceSound(byte[] ba, int chan) {
            float[] f = ToFloatArray(ba);
            audio.clip = AudioClip.Create("", f.Length, chan, FREQUENCY, false);
            audio.clip.SetData(f, 0);
            if (!audio.isPlaying) audio.Play();

        }
        // Used to convert the audio clip float array to bytes
        public byte[] ToByteArray(float[] floatArray) {
            byte[] byteArray = new byte[floatArray.Length * 4];
            int pos = 0;
            /*foreach (float f in floatArray) {
                byte[] data = System.BitConverter.GetBytes(f);
                System.Array.Copy(data, 0, byteArray, pos, 4);
                pos += 4;
            }*/

            for(int i = 0; i < floatArray.Length; i++) {
                byte[] data = System.BitConverter.GetBytes(floatArray[i]);
                System.Array.Copy(data, 0, byteArray, pos, 4);
                pos += 4;
            }

            return byteArray;
        }

        // Used to convert the byte array to float array for the audio clip
        public float[] ToFloatArray(byte[] byteArray) {
            float[] floatArray = new float[byteArray.Length / 4];
            for (int i = 0; i < byteArray.Length; i += 4) {
                floatArray[i / 4] = System.BitConverter.ToSingle(byteArray, i);
            }
            return floatArray;
        }
    }

    public class VoicePacket
    {
        public string entityId;
        public byte[] data;
        public int channels;

        public VoicePacket(byte[] data, int channels, string entityId) {
            this.data = data;
            this.channels = channels;
            this.entityId = entityId;
        }
    }

}