# RE4-PS2-SCENARIO-SMD-TOOL

Extract and repack RE4 OG PS2 scenario SMD files

**Translate from Portuguese Brazil**

Programa destinado a extrair e recompactar os cenários usando somente um arquivo .OBJ;

## Updates

**Update: V.1.3.0**
<br> Foi reformulada a tool nessa versão do programa.
<br> Foi substituído o arquivo 'idxps2scenario' pelo arquivo 'idx_ps2_scenario'
<br> E o arquivo 'idxps2smd' pelo arquivo 'idx_ps2_smd', para ter um conteúdo mais simples de editar.
<br> O conteúdo dentro do OBJ continua o mesmo, então você pode usar os OBJ gerados com a versão anterior do programa.
<br> Agora o conteúdo de 'BoundingBox' é calculado automaticamente, mas isso pode ser desabilitado.
<br> A representação de 'DrawDistance' mudou para 'BBox', no qual os dados são armazenados de uma maneira diferente da anterior.
<br> Agora pode ser usado mais de um arquivo TPL no SMD, e atribuí-los aos BIN, se o usuário da tool preferir.
<br> Feito melhorias na tool, e outras novidades. 

## RE4_PS2_SCENARIO_SMD_TOOL.exe

Programa destinado a extrair e reempacotar os arquivos de cenário .SMD do RE4 OG de PS2;

## Extract:

Use o bat: "RE4_PS2_SCENARIO_SMD_TOOL Extract all scenario SMD.bat"
<br>Nesse exemplo, vou usar o arquivo: r204_004.SMD
<br>Ao extrair, serão gerados os arquivos:
<br>
<br>* r204_004.scenario.idx_ps2_scenario  // arquivo importante de configurações, para o repack usando o .obj;
<br>* r204_004.scenario.idx_ps2_smd // arquivo importante de configurações, para o repack usando os arquivos .bin;
<br>* r204_004.scenario.obj // conteúdo de todo o cenário, esse é o arquivo que você vai editar;
<br>* r204_004.scenario.mtl // arquivo que acompanha o .obj;
<br>* r204_004.scenario.idxmaterial // conteúdo dos materiais (alternativa ao uso do .mtl);
<br>* r204_004.TPL // esse é o arquivo TPL que está dentro do SMD, é onde ficam as texturas dos modelos;
<br>* r204_004_BIN\ // pasta contendo os arquivos .bin;

## Repack:

Existem duas maneiras de fazer o repack.
<br>* Usando o arquivo .idx_ps2_scenario, o repack será feito usando o arquivo .obj;
<br>* Usando o arquivo .idx_ps2_smd, o repack será feito com os arquivos .bin da pasta "r204_004_BIN";

## Repack com .idx_ps2_scenario

Use o bat: "RE4_PS2_SCENARIO_SMD_TOOL Repack all with idx_ps2_scenario.bat"
<br>Nesse exemplo, vou usar o arquivo: "r204_004.scenario.idx_ps2_scenario"
<br> que vai requisitar também os arquivos:
<br> * r204_004.scenario.obj (obrigatório);
<br> * r204_004.TPL (obrigatório);
<br> * r204_004.scenario.mtl OU r204_004.scenario.idxmaterial;

Ao fazer o repack serão gerados os arquivos:
<br> * r204_004.SMD (esse é o arquivo para ser colocado no r204.dat);
<br> * r204_004.scenario.Repack.idx_ps2_smd
<br> * r204_004_REPACK\ //pasta contendo os novos arquivos .BIN; (aviso: ele sobrescreve os arquivos);

## Repack com .idx_ps2_smd

Use o bat: "RE4_PS2_SCENARIO_SMD_TOOL Repack all with idx_ps2_smd.bat"
<br>Nesse exemplo, vou usar o arquivo: "r204_004.scenario.idx_ps2_smd"
<br> Que vai requisitar os arquivos:
<br> * os arquivos .BIN da pasta "r204_004_BIN";
<br> * e o arquivo TPL de nome "r204_004.TPL";

Ao fazer o repack será gerado o arquivo:
<br> * r204_004.SMD (esse é o arquivo para ser colocado no r204.dat);

Nota: esse é o método antigo, no qual se edita os bin individualmente, porém o repack com .idx_ps2_scenario cria novos bin modificados, e um novo .idx_ps2_smd, no qual pode ser usado para fazer esse repack; essa opção é para caso você queira colocar um .bin no .smd que o programa não consiga criar.

## Sobre r204_004.scenario.obj
Esse arquivo é onde está todo o cenário, nele os arquivos BIN são separados por grupos, no qual a nomenclatura deve ser respeitada:
<br> Exemplo:
<br> PS2SCENARIO#SMD_000#SMX_000#TYPE_09#BIN_129#COLOR#
<br> PS2SCENARIO#SMD_077#SMX_060#TYPE_8C#BIN_140#NORMAL#

Sendo:
<br>* É obrigatório o nome do grupo começar com "PS2SCENARIO", e ser dividido por # ou _
<br>* A ordem dos campos não pode ser mudada;
<br>* SMD_000 esse é o ID da posição da Entry/Line no .SMD, a numeração é em decimal
<br>* SMX_000 esse é o ID do SMX, veja o arquivo .SMX,  a numeração é em decimal
<br>* TYPE_08 esse é um valor em hexadecimal que representa flags, veja mais abaixo sobre.
<br>* BIN_000 esse é o id do bin que será usado, para bin repetidos, será considerado somente o primeiro, (o próximo com o mesmo id, será usado o mesmo modelo que do primeiro).
<br>* COLOR/NORMAL esse texto define o tipo de BIN, sendo:
<br>+ COLOR: o BIN não vai ter as normals, mas vai ter cor por vértices;
<br>+ NORMAL: o BIN vai ter as normals, e não terá cor por vértices (esse tipo ocupa mais espaço, pois também tem suporte a pesos por Bone);
<br>* o nome do grupo deve terminar com # ou _ (pois, após salvo o arquivo, o Blender coloca mais texto no final do nome do grupo);

----> Caso use outro editor (3dsMax), o nome dos objetos/grupos também pode ser:
<br> PS2SCENARIO\_SMD\_000\_SMX\_000\_TYPE\_09\_BIN\_129\_COLOR\_
<br> PS2SCENARIO\_SMD\_077\_SMX\_060\_TYPE\_8C\_BIN\_140\_NORMAL\_

----> Sobre verificações de grupos:
<br> * No Repack se ao lado direito do nome do grupo aparecer o texto "The group name is wrong;", significa que o nome do grupo está errado, e o seu arquivo SMD vai ficar errado;
<br> * E se ao lado direito aparecer "Warning: Group not used;" esse grupo está sendo ignorado pelo meu programa. Caso, na verdade, você gostaria de usá-lo, você deve arrumar o nome do grupo;

Nota: No arquivo OBJ a versão atual tem a mesma lógica que a versão antiga do programa. 

**Editando o arquivo .obj no Blender**
<br>No importador de .obj marque a caixa "Split By Group" que está no lado direito da tela.
<br>Com o arquivo importado, cada objeto representa um arquivo .BIN
<br>![Groups](Ps2Groups.png)

**Ao salvar o arquivo**
<br>Marque as caixas "Triangulated Mesh" e "Object Groups" e "Colors".
<br> No arquivo .obj o nome dos grupos vai ficar com "_Mesh" no final do nome (por isso, no editor, termina o nome do grupo com # para evitar problemas);

## Sobre .idx_ps2_scenario / .idx_ps2_smd
Segue abaixo a lista de comandos mais importantes presente no arquivo:

**Configurações Gerais:**
<br> * Magic:0040 // valor do magic é oculto por padrão, e seu valor padrão é 0040, os valores possíveis são 0040 e 0031;
<br> * SmdFileName:r204_004.SMD // esse é o nome do arquivo SMD que será gerado;
<br> * TplFileName:r204_004.TPL // esse é o nome do arquivo TPL que será colocado dentro do SMD;
<br> * BinFolder:r204_004_BIN // esse é o nome da pasta onde serão salvos ou estão os arquivos .BIN;
<br> * UseIdxMaterial:false // caso ativado, será usado o arquivo .idxmaterial ao invés do .mtl para fazer o repack (campo somente no idx_ps2_scenario);
<br> * AutoCalcBoundingBox:true // Calcula automaticamente o BoundingBox de cada arquivo BIN (campo somente no idx_ps2_scenario);
<br> * IgnoreBoundingBox:false // caso ativo, preenche o "BoundingBox" de todos os BIN com o valor 0 (campo somente no idx_ps2_scenario);

**Configuração Por SMD Entry**
<br> * SMD_000 // define um novo SMD entry, onde 000 é o ID do SMD, tudo o que vier abaixo dele vai ser referente a esse SMD entry, até que apareça outro SMD_001, podem estar em qualquer ordem, mas não repita a numeração.
<br> * PositionX:0.0 // posição X do bin na cena. 
<br> * PositionY:0.0 // posição Y do bin na cena. 
<br> * PositionZ:0.0 // posição Z do bin na cena. 
<br> * AngleX:0.0 // ângulo de rotação X do bin na cena.
<br> * AngleY:0.0 // ângulo de rotação Y do bin na cena.
<br> * AngleZ:0.0 // ângulo de rotação Z do bin na cena.
<br> * ScaleX:1.0 // escala X do bin na cena.
<br> * ScaleY:1.0 // escala Y do bin na cena.
<br> * ScaleZ:1.0 // escala Z do bin na cena.
<br> * TplFileID:0 // define o arquivo TPL associado ao arquivo bin, no arquivo 'idx_ps2_scenario' caso o valor desse campo seja zero, esse campo é omitido.
<br> // Por exemplo, se o nome do arquivo TPL for 'r204_004.TPL', para o TplFileID de ID 1 o nome do arquivo deve ser 'r204_004.1.TPL' e o de ID 2 deve ser 'r204_004.2.TPL', os valores aqui são em decimal.

**Os campos abaixo de 'BBox' estão somente presentes no arquivo idx_ps2_scenario**
<br> // para esses campos funcionarem, as tags 'AutoCalcBoundingBox' e 'IgnoreBoundingBox' devem ser definidas como false.
<br> * BBoxMinX:0.0
<br> * BBoxMinY:0.0
<br> * BBoxMinZ:0.0
<br> * BBoxMaxX:0.0
<br> * BBoxMaxY:0.0
<br> * BBoxMaxZ:0.0
<br> // os valores estão em escala global do modelo e representam os vértices que estão nos limites do modelo de cada bin.

**Os campos abaixo somente estão presentes no arquivo idx_ps2_smd**
<br> * BinFileID:0 // diz qual arquivo bin é usado (valor em decimal).
<br> * SmxID:0 // diz qual SMX é usado e é vinculado ao arquivo SMX (valor em decimal).
<br> * ObjectStatus:08 // valor em hexadecimal, é o mesmo que o campo "TYPE" no OBJ, veja mais abaixo sobre;
<br> // os campos abaixo são ocultos por padrão:
<br> * PositionW:1.0 // sempre 1.0
<br> * AngleW:1.0 // sempre 1.0
<br> * ScaleW:1.0 // sempre 1.0
<br> * FixedFF:FF // sempre FF valor em hexadecimal
<br> * Unused1:0 // sempre 0 valor em hexadecimal
<br> * Unused2:0 // sempre 0 valor em hexadecimal

 ## sobre ObjectStatus / TYPE
Esse campo é um enum bitflag, isso significa que cada bit tem uma função, segue abaixo do que cada um faz:
 <br> * 0x00 / 0b00000000 // nenhuma flag ativada.
 <br> * 0x?1 / 0b?0001 // "EXE Scripted", contém evento associado ao modelo, não o remova do SMD
 <br> * 0x?2 / 0b?0010 // desconhecido, não usado no jogo.
 <br> * 0x?4 / 0b?0100 // "Assign SMX Group ID"
 <br> * 0x?8 / 0b?1000 // "Ends SMX Group, Has SMX(?)"
 <br> * 0x1? / 0b0001? // "Use BIN from shared SMD (BLK)", isso é quando tem BIN compartilhado.
 <br> * 0x2? / 0b0010? // "Use TPL from shared SMD (BLK)", não testei isso, acho que minha tool não suporta isso.
 <br> * 0x4? / 0b0100? // Use MOT from shared SMD (BLK), MOT não funciona / não é usado.
 <br> * 0x8? / 0b1000? // Desconhecido, mas é ativado quando o bin é do tipo NORMAL.
 <br> // Outros valores são combinações dessas funcionalidades, veja bit a bit para saber o que faz.

## sobre COLOR vs NORMAL
 A maioria dos BINS são do tipo color, e funciona em todos os cenários, aqui a cor serve para representar o efeito da normal, que não existe no arquivo.
 <br>Para o tipo Normal funcionar, além de definir como NORMAL, uma tag no arquivo SMX deve ser ativa, e mesmo assim pode não funcionar corretamente, Só vi funcionar corretamente no cenário 'r20e_004', já por exemplo no "r100_004" não funcionou corretamente.
 <br> Nota: a flag no arquivo SMX é "AlphaHierarchy" e o valor é 0x8?, lembrando que esse campo é um enum bitflag; 

 ## Materials/Texturas no MTL

Agora, é no arquivo MTL que é onde são definidos quais são os IDs (índices do TPL) das texturas usadas no arquivo BIN;
<br>O nome da textura tem que ser somente um número que é o ID da textura;
<br>O arquivo MTL espera que essas texturas estejam em uma pasta com o mesmo nome do arquivo BIN que foi extraído;
<br>Para extrair as texturas do arquivo TPL use o programa [RE4-PS2-TPL-TOOL](https://github.com/JADERLINK/RE4-PS2-TPL-TOOL) a partir da versão B.1.1;
<br> Use o Bat: "RE4_PS2_TPL_EXTRACT_To_Scenario_TPL.bat", para extrair as texturas corretamente;
<br> Atenção: lembre de fazer o repack do TPL antes de fazer o repack do SMD;

## Sobre BoundingBox / BBox
Agora, os campos de BoundingBox são calculados automaticamente por padrão, então você não vai presisar se preocupar com isso.
<br>Caso o seu modelo fique muito longe da posição original, ele pode começar a aparecer e desaparecer, isso é devido aos valores que definem a que distância os modelos iram ser vistos.
<br> Mude os valores de "BBoxMin*" para -327670 ou 0
<br> E os valores de "BBoxMax*:" para 327670 ou 0
<br> Isso vai garantir que o modelo fique visível, porém pode gerar um bug na iluminação (ou não).

## Bugs e Infos
Ao mudar os valores originais dos campos "Scale*", "Angle*", "Position*", "BBoxMin*" e "BBoxMax*", pode ocasionar um bug na iluminação do modelo, no qual não sei como resolver.
<br> Nota: O programa muda o valor de "Position*" automaticamente, pois tenho que centralizar os arquivos BIN, então todas as SMD entry com BIN repetidos, você vai ter que recalcular a posição do objeto;
<br> As novas posições ficam no arquivo "r204_004.scenario.Repack.idx_ps2_smd";

## Código de terceiro:
[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader):
Encontra-se em "RE4_PS2_SCENARIO_SMD_TOOL\\CjClutter.ObjLoader.Loader", código modificado, as modificações podem ser vistas aqui: [link](https://github.com/JADERLINK/ObjLoader).

**At.te: JADERLINK**
<br>2025-10-19