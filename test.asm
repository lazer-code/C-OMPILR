[ds+8]
mov [ds+16], $25
mov [ds+24], $28
cmp [ds+16], [ds+24]
jg setJumpAddress
mov eax, $10
ret

setJumpAddress:
    mov eax, $15
    ret