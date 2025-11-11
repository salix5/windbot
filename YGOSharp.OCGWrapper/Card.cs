using YGOSharp.OCGWrapper.Enums;
using System.Data;

namespace YGOSharp.OCGWrapper
{
    public class Card
    {
        const int CARD_ARTWORK_VERSIONS_OFFSET = 20;
        const int CARD_BLACK_LUSTER_SOLDIER2 = 5405695;
        public struct CardData
        {
            public int Code;
            public int Alias;
            public long Setcode;
            public long Type;
            public int Level;
            public int Attack;
            public int Defense;
            public int LScale;
            public int RScale;
            public int LinkMarker;
            public long Attribute;
            public long Race;
        }

        public int Id { get; private set; }
        public int Ot { get; private set; }
        public int Alias { get; private set; }
        public long Setcode { get; private set; }
        public long Type { get; private set; }

        public int Level { get; private set; }
        public int LScale { get; private set; }
        public int RScale { get; private set; }
        public int LinkMarker { get; private set; }

        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public long Attribute { get; private set; }
        public long Race { get; private set; }

        internal CardData Data { get; private set; }

        public static Card Get(int id)
        {
            return CardsManager.GetCard(id);
        }

        public bool HasType(CardType type)
        {
            return (Type & (long)type) != 0;
        }

        public bool HasSetcode(int value)
        {
            int settype = value & 0x0fff;
            int setsubtype = value & 0xf000;
            for (int i = 0; i < 4; i++)
            {
                long section = (Setcode >> (i * 16)) & 0xffffL;
                if ((section & 0x0fff) == settype && (section & setsubtype) == setsubtype)
                    return true;
            }
            return false;
        }

        public bool IsExtraCard()
        {
            return HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz) || HasType(CardType.Link);
        }

        public bool IsAlternative()
        {
            if (Data.Code == CARD_BLACK_LUSTER_SOLDIER2)
                return false;
            return Data.Alias > 0 && (Data.Alias < Data.Code + CARD_ARTWORK_VERSIONS_OFFSET) && (Data.Code < Data.Alias + CARD_ARTWORK_VERSIONS_OFFSET);
        }

        internal Card(IDataRecord reader)
        {
            Id = reader.GetInt32(0);
            Ot = reader.GetInt32(1);
            Alias = reader.GetInt32(2);
            Setcode = reader.GetInt64(3);
            Type = reader.GetInt64(4);

            int levelInfo = reader.GetInt32(5);
            Level = levelInfo & 0xffff;
            LScale = (levelInfo >> 24) & 0xff;
            RScale = (levelInfo >> 16) & 0xff;

            Race = reader.GetInt64(6);
            Attribute = reader.GetInt64(7);
            Attack = reader.GetInt32(8);
            Defense = reader.GetInt32(9);

            if (HasType(CardType.Link))
            {
                LinkMarker = Defense;
                Defense = 0;
            }

            Data = new CardData()
            {
                Code = Id,
                Alias = Alias,
                Setcode = Setcode,
                Type = Type,
                Level = Level,
                Attribute = Attribute,
                Race = Race,
                Attack = Attack,
                Defense = Defense,
                LScale = LScale,
                RScale = RScale,
                LinkMarker = LinkMarker
            };
        }
    }
}