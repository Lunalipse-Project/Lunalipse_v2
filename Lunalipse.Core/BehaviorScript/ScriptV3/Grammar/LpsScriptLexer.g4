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
DO_BUILDIN_FUNC: [Cc] 'asting a magic spell called ';
PROGRAM_GROUP_START: 'I made up an checklist with the following items';
PROGRAM_GROUP_END: 'and that is all about checklist of ';
DO_ACTIONS: 'I start checking the checklist called ';
INDEXING_PREFIX: 'I ask ';
INDEXING_INDEX: ' for the stuff at position of ';
ASSIGN_PREFIX: 'I told ';
MAKE_CONSTANT: 'that I have made a Pinkie promise';
ASSIGN_TO: ' about ';
PARAMETER_SPLIT: ' with constraints';
TRUE: 'true';
FALSE: 'false';

DO: 'I do the following';
IF: 'when ';
IF_SUFX: ' becomes true';
ELSE: 'otherwise it should be';
END_IF: 'that is the decision I made';
BREAK: 'I want to stop here';
LOOP: 'I started a loop';
END_LOOP: 'that is my loop';

END_PROGRAM: 'And that is my adventure in today';
AUTHUR_NAME: 'Your loyal citizen:';

DECLR: 'I have ';

FULLSTOP: '.';
COLON: ':';
WITH_ACTION: 'for ';
VOLUM: ' percent of awesomeness';
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
GR: 'greater than';
LS: 'less than';
GE: 'greater than or equals to';
LE: 'less than or equals to';
EQ: 'equals to';
NEQ: 'not equals to';

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
