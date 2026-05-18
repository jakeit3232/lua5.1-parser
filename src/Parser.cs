using System;
using System.Globalization;
using System.IO;
using System.Security;

class header
{
    public string signature;
    public byte version; // 0x51 
    public byte format;
}


class Parser
{
    
    enum opmode { iABC, iABx, iAsBx }
    static readonly opmode[] opmodes =
    {
        opmode.iABC,  opmode.iABx,  opmode.iABC,  opmode.iABC,  opmode.iABC,
        opmode.iABx,  opmode.iABC,  opmode.iABx,  opmode.iABC,  opmode.iABC,
        opmode.iABC,  opmode.iABC,  opmode.iABC,  opmode.iABC,  opmode.iABC,
        opmode.iABC,  opmode.iABC,  opmode.iABC,  opmode.iABC,  opmode.iABC,
        opmode.iABC,  opmode.iABC,  opmode.iAsBx, opmode.iABC,  opmode.iABC,
        opmode.iABC,  opmode.iABC,  opmode.iABC,  opmode.iABC,  opmode.iABC,
        opmode.iABC,  opmode.iAsBx, opmode.iAsBx, opmode.iABC,  opmode.iABC,
        opmode.iABC,  opmode.iABx,  opmode.iABC
    };
    static readonly string[] opcodes =
    {
        "MOVE", "LOADK", "LOADBOOL", "LOADNIL", "GETUPVAL",
        "GETGLOBAL", "GETTABLE", "SETGLOBAL", "SETUPVAL", "SETTABLE",
        "NEWTABLE", "SELF", "ADD", "SUB", "MUL",
        "DIV", "MOD", "POW", "UNM", "NOT",
        "LEN", "CONCAT", "JMP", "EQ", "LT",
        "LE", "TEST", "TESTSET", "CALL", "TAILCALL",
        "RETURN", "FORLOOP", "FORPREP", "TFORLOOP", "SETLIST",
        "CLOSE", "CLOSURE", "VARARG", "JAKE"
    };

    public static void parser(byte[] luac)
    {
        Console.WriteLine("disassembly:");
        int i = 0;
        
        // signature (4 bytes)
        string sig = System.Text.Encoding.ASCII.GetString(luac, i, 4);
        i += 4;

        // version (0x51)
        byte version = luac[i++];

        // format (1 byte)
        byte format = luac[i++];

        // endianness (0 = big 1 = little)
        byte endian = luac[i++];

        // size  (1 byte each..)
        byte intSize = luac[i++];
        byte sizeTSize = luac[i++];
        byte instrSize = luac[i++];
        byte numberSize = luac[i++];
        byte integralFlag = luac[i++];

        // proto
        long srcLen = BitConverter.ToInt64(luac, i);
        i += sizeTSize;
        i += (int)srcLen;

        i += intSize;
        i += intSize;

        i += 4;

        int codeCount = BitConverter.ToInt32(luac, i);
        i += intSize;

        var code = new uint[codeCount];
        for (int k = 0; k < codeCount; k++)
        {
            code[k] = BitConverter.ToUInt32(luac, i);
            i += instrSize;
        }

        int constCount = BitConverter.ToInt32(luac, i);
        i += intSize;

        var constants = new object[constCount];
        for (int k = 0; k < constCount; k++)
        {
            byte type = luac[i++];
            switch (type)
            {
                // NIL
                case 0: constants[k] = null; break;
                // BOOL
                case 1: constants[k] = luac[i++] != 0; break;
                // NUMBER
                case 3:
                    constants[k] = BitConverter.ToDouble(luac, i);
                    i += numberSize;
                    break;
                // STRING
                case 4:
                    long len = BitConverter.ToInt64(luac, i);
                    i += sizeTSize;
                    constants[k] = System.Text.Encoding.ASCII.GetString(luac, i, (int)len - 1);
                    i += (int)len;
                    break;
            }
        }

        foreach (var instr in code)
            decode(instr, constants);
        
        Console.WriteLine();
        int count = 0;
        foreach (var c in constants)
        {
            Console.WriteLine($"k[{count}]: {c}");
            count++;
        }



        static void decode(uint instr, object[] k)
        {
            int opcode = (int)(instr & 0x3F);
            int a    = (int)((instr >> 6) & 0xFF);
            int c    = (int)((instr >> 14) & 0x1FF);
            int b    = (int)((instr >> 23) & 0x1FF);
            int bx   = (int)((instr >> 14) & 0x3FFFF);
            int sbx  = bx - 131071;

            string name = opcodes[opcode];

            switch (opmodes[opcode])
            {
                case opmode.iABx:
                    Console.WriteLine($"{name,-10} R{a}, K[{bx}] ; {k[bx]}");
                    break;
                case opmode.iAsBx:
                    Console.WriteLine($"{name,-10} R{a}, {sbx:+#;-#;0}");
                    break;
                default:
                    Console.WriteLine($"{name,-10} A={a}, B={b}, C={c}");
                    break;
            }
        }
    }
}