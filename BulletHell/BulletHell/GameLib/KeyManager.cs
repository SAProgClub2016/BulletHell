using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BulletHell.GameLib
{
    using Timer = BulletHell.Time.Timer;
    public delegate void KeyPressed(KeyManager km, Keys key, bool repeat=false);
    public delegate void KeyReleased(KeyManager km, Keys key);
    
    public class KeyIndexedArray<T>
    {
        T[] vals;
        bool canSet;
        Func<T, T> valF;

        public KeyIndexedArray(T[] wrap, bool setEnabled = false)
            : this(wrap, Utils.Identity<T>, setEnabled)
        {
        }

        public KeyIndexedArray(T[] wrap, Func<T,T> validate, bool setEnabled = true)
        {
            vals = wrap;
            canSet = setEnabled;
            valF = validate;
        }

        public T this[int kCode]
        {
            get
            {
                return vals[kCode];
            }
            set
            {
                if (canSet)
                {
                    vals[kCode] = valF(value);
                }
            }
        }
        public T this[Keys kCode]
        {
            get
            {
                return this[(int)kCode];
            }
            set
            {
                this[(int)kCode] = value;
            }
        }
    }

    public class KeyManager
    {
        public const int NUMKEYS = 256;
        public const int NO_REPEAT = -1;
        public const int DEFAULT_REPEAT = 0;
        private bool[] keyPressed;
        private int[] repeats;
        private Timer[] timers;
        private KeyPressed[] pressHandlers;
        private KeyReleased[] releaseHandlers;

        private KeyIndexedArray<bool> pubKP;
        private KeyIndexedArray<int> pubRep;
        private KeyIndexedArray<KeyPressed> pubPress;
        private KeyIndexedArray<KeyReleased> pubRel;
        
        public KeyManager()
        {
            keyPressed = new bool[NUMKEYS];
            repeats = new int[NUMKEYS];
            timers = new Timer[NUMKEYS];
            pressHandlers = new KeyPressed[NUMKEYS];
            releaseHandlers = new KeyReleased[NUMKEYS];
            for (int i = 0; i < NUMKEYS; i++)
            {
                repeats[i] = NO_REPEAT;
                keyPressed[i] = false;
                timers[i] = new Timer();
                pressHandlers[i] = this.EmptyKeypressHandler;
                releaseHandlers[i] = this.EmptyKeyreleaseHandler;
            }
            pubKP = new KeyIndexedArray<bool>(keyPressed);
            pubRep = new KeyIndexedArray<int>(repeats, RepValidator);
            pubPress = new KeyIndexedArray<KeyPressed>(pressHandlers, KPValidator);
            pubRel = new KeyIndexedArray<KeyReleased>(releaseHandlers, KRValidator);
        }

        private KeyReleased KRValidator(KeyReleased r)
        {
            return r ?? this.EmptyKeyreleaseHandler;
        }

        private KeyPressed KPValidator(KeyPressed p)
        {
            return p ?? this.EmptyKeypressHandler;
        }

        private int RepValidator(int i)
        {
            return Math.Max(i, -1);
        }

        public void KeyPressed(Keys key)
        {
            int k = (int)key;
            bool rep = keyPressed[k];
            keyPressed[k] = true;
            int repe = repeats[k];
            if(repe == NO_REPEAT)
            {
                if(!rep)
                {
                    pressHandlers[k](this, key);
                }
                return;
            }
            if(repe == DEFAULT_REPEAT)
            {
                pressHandlers[k](this, key, rep);
                return;
            }
            if(!rep || timers[k].Time>repe)
            {
                pressHandlers[k](this, key, rep);
                timers[k].Reset();
            }
        }
        public void KeyReleased(Keys key)
        {
            keyPressed[(int)key] = false;
            releaseHandlers[(int)key](this, key);
        }

        private void EmptyKeyreleaseHandler(KeyManager km, Keys key) { }
        private void EmptyKeypressHandler(KeyManager km, Keys key, bool repeat) { }

        public bool this[int kCode]
        {
            get
            {
                return keyPressed[kCode];
            }
        }
        public bool this[Keys kCode]
        {
            get
            {
                return this[(int)kCode];
            }
        }
        public KeyIndexedArray<bool> KeyState
        {
            get
            {
                return pubKP;
            }
        }
        public KeyIndexedArray<int> Repeat
        {
            get
            {
                return pubRep;
            }
        }
        public KeyIndexedArray<KeyPressed> OnPress
        {
            get
            {
                return pubPress;
            }
        }
        public KeyIndexedArray<KeyReleased> OnRelease
        {
            get
            {
                return pubRel;
            }
        }
    }
}
