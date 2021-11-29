$i = 1
 
while ($i -le 28) {
 
    Rename-Item ".\ac ($i).png" "ac$i.png"
    $i += 1
}
