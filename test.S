_main:

mov [ds+16], $25
mov [ds+24], $28
cmp [ds+24], [ds+16]
jg L1
mov eax, $10
ret 
L1: 
mov eax, $15
ret 
