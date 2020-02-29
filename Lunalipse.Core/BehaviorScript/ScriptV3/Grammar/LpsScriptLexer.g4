lexer grammar LpsScriptLexer;

options {
    language=Java;
}

fragment DIGIT : [0-9];
fragment LETTER : [A-Za-z];
fragment WhiteSpace:[ \r\t\n];
fragment NewLine: [\n\r];

PROG_START: 'Dear Princess Luna:' -> type(PROG_START);
PROG_NAME_DECLARE: 'I would like to tell you my adventure of ';
CATALOGUE_CHOOSE: 'For the stuff in ';
PLAY: 'I play music called ';
PLAY_NUM: 'I play music numbered ';
SET_EQULAIZER: 'And I would like to set my equalizer using ';
DO_BUILDIN_FUNC: [Cc] 'sting a magic spell called ';
PROGRAM_GROUP_START: 'I made up an checklist with the following items';
PROGRAM_GROUP_END: 'and that is all about checklist of ';
DO_ACTIONS: 'I start checking the checklist called ';
INDEXING_PREFIX: 'I ask ';
INDEXING_INDEX: ' for his stuff at position of ';
ASSIGN_PREFIX: 'I told ';
MAKE_CONSTANT: 'that I have made a Pinkie promise';
ASSIGN_TO: ' about ';
PARAMETER_SPLIT: ' with constraints:';

END_PROGRAM: 'And that is my adventure in today';
AUTHUR_NAME: 'Your loyal citizen:';


FULLSTOP: '.';
WITH_ACTION: 'for ';
VOLUM: ' precent of awesomeness';
REPEAT_TIMES: ' times';

LSQUARE: '[';
RSQUARE: ']';
LPAREN: '(';
RPAREN: ')';
COMMA: ',';
ADD: '+';
MINUS: '-';
MULT: '*';
DIV : '/';
AND: 'and';
OR: 'or';
NOT: 'not';
GR: 'larger than';
LS: 'less than';
GE: 'larger than or equal to';
LE: 'less than or to';
EQ: 'equal to';

RQUOTE: '"' -> more, mode(STR);
INT: DIGIT+;
REAL: INT'.'INT;
ID : LETTER(LETTER|DIGIT|'_')*;
COMMENT_SINGLE: 'P.S.' -> skip,mode(CMT_SNGL);
COMMENT_MULTI: 'Here my long waffle start:' -> skip,mode(CMT_LONG);
WS     : WhiteSpace+    -> skip;


mode CMT_SNGL;
NEW_LINE: [\r\n] -> skip,mode(DEFAULT_MODE);
DONT_CARE1: . -> skip;

mode CMT_LONG;
END: 'And my long waffle end.' -> skip,mode(DEFAULT_MODE);
DONT_CARE2: . -> skip;

mode STR;
STRING: '"' -> mode(DEFAULT_MODE);
Wspace: NewLine+ -> skip;
TEXT: . -> more;