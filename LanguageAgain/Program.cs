using LanguageAgain;

string src = """

import io;
import collections;
import console;

let type StructureInstance = struct {
    int x, int y, int r,
}

let type Structure = struct {
    let myList = collections::List<T> {  };

    struct { int x, int y }[]* footprint,
    char[]* name,
    void(StructureInstance) updateInstance,
};

Structure Conveyor = Structure {
    [{0, 0}],
    "Conveyor",
    void(StructureInstance instance) {
        let x = 10;
    },
}

let void() x = {

}

int(int a, int b) add = {

}

type Expression = struct {
    int Type,
}

type BinaryExpression = struct {
    int Type
}

void(Structure*, int x, int y, int r) place = void(Structure* structure, int x, int y, int r) {
    StructureInstance instance = { structure, x, y, r }

    structure.updateInstance(instance);
}

let type List = void(type T) { 
    return struct {
        var int length,
        var T* elements,
    }
}

List(int) x = 0;

char[] source = "";

int PIN_ON = 1;
int* PIN_0 = 0x10;

let main = {
    console::print "hello world"
}

let main = &{
    
    goto main
}

""";


// main.fc
string src2 = """
// module program;

// import console;

// import libc;

let DayOfWeek = enum {
    Mond = 1 << next;
    Tues = 1 << next;
    Weds = 1 << next;
    Thur = 1 << next;
    Frid = 1 << next;
    Satu = 1 << next;
    Sund = 1 << next; 
}

let type Token = struct {
    int start;
    int end;
};

let Expression = union {
    let Binary = struct { 
        var int operation;
        var Expression* left;
        var Expression* right;
    };
    let Number = struct {
        var Token value;
    };
};

let Point = struct {
    var int x;
    var int y;

    let to_string;
};

let Slice = struct {
    let void* start;
    let ptr size;
}

let Slice::to_ptr = void*(Slice slice) {
    return start;
};

let add = Point(let Point a, let Point b) {
    return Point { x = a.x + b.x; y = a.y + b.y; };
};

let main = int() {
     
    const int x = 0;
};
""";

string lexerFc = """
module fcc::lexer;

let TokenKind = enum {
    let identifier;
    let Void;
};

let Token = struct {
    let TokenKind kind;
    let int start;
    let int end;
};

let consume_token = void(let uint8[]* str) {
    let Token token = Token { 
        kind = TokenKind::identifier; 
    };
};

""";

SourceReader sr = new(File.ReadAllText("C:\\Users\\Ryan\\source\\repos\\LanguageAgain\\LanguageAgain\\code\\program.fc"));
List<Token> tokens = [];
Token token = Lexer.ReadToken(sr);
while (token.kind != TokenKind.EndOfSource)
{
    tokens.Add(token);
    token = Lexer.ReadToken(sr);
}
tokens.Add(token);

var compilationUnit = Parser.ParseCompilationUnit(new(tokens.ToArray()));

Interpreter interpreter = new(compilationUnit);

interpreter.RunFunction("main");
