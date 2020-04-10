parser grammar LpsScriptParser;
options {
    tokenVocab=LpsScriptLexer;
    language=Java;
}

prg: prg_start prg_content prg_end;

prg_start: PROG_START PROG_NAME_DECLARE ID FULLSTOP;
prg_end: END_PROGRAM FULLSTOP AUTHUR_NAME STRING FULLSTOP;

prg_content: declares group_statemnt FULLSTOP prg_content
            |declares statements
            | ;

statements: statements statement
            |
            ;

if_branch: DO COLON statements IF expr_wrap IF_SUFX COMMA (else_branch)? END_IF;
else_branch: ELSE COLON statements;

loop: LOOP COLON statements END_LOOP;

declares: declares declare
          |declare
          |;

declare: DECLR declare_ids FULLSTOP;

declare_ids: ID
            |declare_ids COMMA declare_ids;

statement: cata_choose FULLSTOP					
         | play_actions conditions? FULLSTOP	
         | set_eqzr FULLSTOP					
         | do_action conditions? FULLSTOP       
         | assign FULLSTOP                      
         | func_call conditions? FULLSTOP
         | if_branch FULLSTOP
         | BREAK FULLSTOP   
         | loop FULLSTOP
         ;

cata_choose: CATALOGUE_CHOOSE any_id;
play_actions: PLAY STRING
            | PLAY_NUM expr_wrap;
set_eqzr: SET_EQULAIZER (array|ID);
do_action: DO_ACTIONS ID;

group_statemnt: PROGRAM_GROUP_START COLON statements PROGRAM_GROUP_END ID;

loop_act: WITH_ACTION expr_wrap REPEAT_TIMES			# SuffixLoop;
volume_set: WITH_ACTION expr_wrap VOLUM		            # SuffixSetVol;

conditions: condition
		  | conditions COMMA conditions;

condition: loop_act									
          |volume_set
          ;

assign: ASSIGN_PREFIX ID MAKE_CONSTANT? ASSIGN_TO expr_wrap;

expr_wrap : expr;

expr:  optr_P0 expr                 #exprUnary
      |expr optr_P1 expr            #exprP1
      |expr optr_P2 expr            #exprP2
      |expr optr_P3 expr            #exprP3
      |expr optr_P4 expr            #exprP4
      |expr optr_P5 expr            #exprP5
      |expr optr_P6 expr            #exprP6
      |func_call                    #exprInd
      |array_indexing               #exprInd
      |array                        #exprInd
      |any                          #exprInd
      |LPAREN expr RPAREN           #exprParen;

func_call: DO_BUILDIN_FUNC ID PARAMETER_SPLIT COLON array
         | DO_BUILDIN_FUNC ID;

array_indexing: INDEXING_PREFIX ID INDEXING_INDEX (ID|INT);

array: LSQUARE array_content RSQUARE                # ArrayDeclr
        ;				
array_content:  expr_wrap		                            							
              | array_content COMMA array_content
              | ;

any: any_id|any_number;
any_id: ID|STRING;
any_number: REAL|INT;
optr_P0: NOT|ADD|MINUS;
optr_P1: MULT|DIV;
optr_P2: ADD|MINUS;
optr_P3: GR|LS|GE|LE;
optr_P4: EQ|NEQ;
optr_P5: AND;
optr_P6: OR;