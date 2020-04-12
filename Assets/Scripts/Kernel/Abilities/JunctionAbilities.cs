﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVIII
{
    namespace Kernel
    {
        /// <summary>
        /// Junction Abilities Data
        /// </summary>
        /// <see cref="https://github.com/alexfilth/doomtrain/wiki/Junction-abilities"/>
        public sealed class JunctionAbilities : IAbility
        {
            #region Fields

            /// <summary>
            /// Section Count
            /// </summary>
            public const int Count = 20;

            /// <summary>
            /// Section ID
            /// </summary>
            public const int ID = 11;

            #endregion Fields

            #region Constructors

            private JunctionAbilities(BinaryReader br, int i)
            {
                //0x0000	2 bytes
                Name = Memory.Strings.Read(Strings.FileID.Kernel, ID, i * 2);
                //0x0002	2 bytes
                Description = Memory.Strings.Read(Strings.FileID.Kernel, ID, i * 2 + 1);
                br.BaseStream.Seek(4, SeekOrigin.Current);
                //0x0004  1 byte
                AP = br.ReadByte();
                //0x0005  3 byte J_Flag
                var tmp = br.ReadBytes(3);
                Flags = (JunctionAbilityFlags)(tmp[2] << 16 | tmp[1] << 8 | tmp[0]);
            }

            #endregion Constructors

            #region Properties

            public byte AP { get; }

            public FF8String Description { get; }

            /// <summary>
            /// Flags related to what abilities are enabled
            /// </summary>
            public JunctionAbilityFlags Flags { get; }

            public Icons.ID Icon { get; } = Icons.ID.Ability_Junction;

            public FF8String Name { get; }

            public byte Palette { get; } = Ability.DefaultPalette;

            #endregion Properties

            #region Methods

            public static Dictionary<Abilities, JunctionAbilities> Read(BinaryReader br)

                                                    => Enumerable.Range(0, Count)
                    .ToDictionary(i => (Abilities)i, i => CreateInstance(br, i));

            private static JunctionAbilities CreateInstance(BinaryReader br, int i)
                => new JunctionAbilities(br, i);

            #endregion Methods
        }
    }
}