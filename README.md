# RE4 SMD PS2 Tool

Extract and repack ps2 re4 smd files

Translate from Portuguese Brazil

Esse são os códigos dos programas destinados a extrair e recompactar os arquivos SMD da versão de PS2.
<br>Essa é a primeira versão, caso achar algum bug favor reportar.

**versão alfa.1.1.0.0:**

Atualizado o "SMD_PS2_Extractor.exe", o "SMD_PS2_Repack.exe" continua igual pode ser usado o mesmo.
<br>Mudanças realizadas no SMD_PS2_Extractor:
 - Agora usa de base a versão alfa.1.2.0.0 do BINdecoderTest
 - então vai gerar os modelos 100x menor que a versão anterir.
 - vai criar os arquivos: "rxxx_Model.mtl", "rxxx_Model_0.01.obj", "rxxx_Model_1.00.obj", "rxxx_Reference_0.01.smd", "rxxx_Reference_1.00.smd".
  - * Model_1.00 é o tamanho real do cenário e Model_0.01 é 100x menor, que é a mesma escala dos .bin extraídos.
  - * os arquivos "rxxx_Reference*" contem triângulos com as posições dos .bin definidos no arquivo .smd(re4).
  - use o arquivo .bat "SMD_PS2_Extractor - Only models.bat" para criar somente os arquivos "rxxx_Model*" e "rxxx_Reference*", sem extrair os outros arquivos. (use para conferir o modelo como um todo)
  - o arquivo .bat "SMD_PS2_Extractor.bat", extrai todos os arquivos.

## SMD_PS2_Extractor

Responsável por extrair os arquivos do SMD.
<br>Ele ira criar o arquivo "idxSmdPs2" que é um arquivo de texto.
<br>Criara uma pasta com nome do arquivo smd, e nela vai ter os arquivos bin's enumerados, e o arquivo tpl.

## SMD_PS2_Repack

Responsável por recompactar o Smd, o arquivo de usado de referência é o idxSmdPs2.

## Arquivo "idxSmdPs2"

Explicação do arquivo, vou usar como exemplo o arquivo "r100_004.SMD" como referência, ao extrair vai gerar o arquivo "r100_004.idxSmdPs2" uma pasta com o nome "r100_004" que vai ter os arquivos "r100.TPL" e "r100_*.BIN" (onde * é um número) [nota: terá mais duas subpastas "Models" e "DebugInfo", mas seus conteúdos são ignorados pelo Repack]

**Conteúdo do idxSmdPs2**
<br>Nota: o Conteúdo com \\\\ é informativo e não existe no arquivo original
```
:##SMD_PS2_Extractor##
:##version A.1.0.0.0##

// o 000 é o id do SMD, sendo a sequencia 000, 001, 002, etc
//BinID: valor decimal, é o id do bin (tipo ushort)
000_BinID:129
//FixedFF: valor hexadecimal, sempre 0xFF (tipo byte)
000_FixedFF:FF
//SmxID: valor decimal, é o id do smx (tipo byte)
000_SmxID:0
//unused1: valor hexadecimal, sempre zero (tipo uint)
000_unused1:00000000
//objectStatus: valor hexadecimal, representa o status do modelo (tipo uint)
000_objectStatus:00000009
//unused2: valor hexadecimal, sempre zero (tipo uint)
000_unused2:00000000
// todos os campos abaixo são do tipo float
// (nota: mantenha os campos W com valor 1)
000_positionX:-115983.234
000_positionY:0
000_positionZ:0
000_positionW:1
000_angleX:0
000_angleY:0
000_angleZ:0
000_angleW:1
000_scaleX:10
000_scaleY:10
000_scaleZ:10
000_scaleW:1

// conteúdo omitido

// quantidade de entradas(linhas) SMD, é referente aos dados informados acima.
SmdCount:283
// quantidade de arquivos bin presentes no arquivo SMD
BinCount:148
//o caminho e início do nome dos arquivos bins (siga o padrão da pasta)
BinBaseName:r100_004\r100_
// informa o local do arquivo TPL
TplFilePatch:r100_004\r100.TPL

```
**Referente ao campo objectStatus**

Nos arquivos originais contem os seguintes valores:
```
00000009
00000008
0000000C
0000000D
0000008C
00000088
00000000
00000001
00000089
0000008D
00000080
```

## SketchUp e as rotações

No arquivo "r100_Model.obj" aplicando a rotação do SMD, faz com que o .obj não carregue no programa, só vai carregar somente se não tiver rotação, ou somente a rotação de um angulo.
<br>Mas o arquivo pode ser aberto nos programas "blender" e "Metasequoia 4"

## Arquivos não suportados

Segue a lista de SMD, que não funcionam com os programas:

* r313_004.SMD
* r314_004.SMD
<br>Motivo: header diferente do padrão, (possível bin diferente)
* r100_01_000.SMD
<br>Motivo: header diferente, e as SMDlines são diferentes, tamanho 72 bytes;
* r100_00_000.SMD
* r100_02_000.SMD
* r100_03_000.SMD
* r100_04_000.SMD
<br>Motivo: da para extrair o smd, porem os arquivos bin são diferentes, (a parte do "VertexLine" tem 32 bytes em vez de 24);

**Adendo:**
* r322_004.SMD
* r323_004.SMD
<br>funcionam, porem os dois primeiros bytes são 31-00 em vez de 40-00.

---
At.te: JADERLINK
<br> 2023-03-12