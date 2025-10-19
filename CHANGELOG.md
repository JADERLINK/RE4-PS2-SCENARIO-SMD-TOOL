# Change Log

Aqui consta o conteúdo das alterações anteriores à V.1.3.0:

**Update: B.1.2.5**
<br> Correção: Quando a quantidade de "segments" ultrapassar o limite permitido por material, agora vai ser criado um novo material. (Não vão mais faltar vértices no modelo);
<br> Nota: O jogo tem um limite de quantos vértices ele consegue carregar/processar;
<br> Melhoria: melhorado a velocidade do repack, agora é muito rápido fazer o repack.
<br> Ao extrair arquivos SMD inválidos, não vai mais ser gerada uma pasta.
<br> E foram feitas melhorias gerais no código;

**Update: B.1.2.04**
<br>Arrumado a compatibilidade com alguns arquivos SMD, por exemplo, o SMD do cenário r215;
<br>Agora, ao extrair o arquivo .bin as "normals" serão normalizadas em vez de ser dividido por um valor padrão;
<br> Ao fazer repack as normals do arquivo .obj serão normalizadas para evitar erros.
<br> O programa, ao gerar o arquivo .obj, não terá mais os zeros não significativos dos números, mudança feita para gerar arquivos menores.
<br> Arrumado o suporte para carregar cores de vértice do arquivo .obj, agora a conversão das cores é feita diferente do que era feito na versão anterior; 

**Update: B.1.2.0.3**
<br>Arrumado bug ao carregar o arquivo .idxmaterial;

**Update: B.1.2.0.2**
<br>Corrigido bug no qual não era rotacionado as normals dos modelos que têm rotação, então, caso esteja usando um .obj de versões anteriores, recalcule as normals;

**Update: B.1.2.0.1**
<br>Corrigido bug que crashava o programa, agora, ao ter material sem a textura principal "map_Kd", será preenchido o Id da textura como 0000; 

### Update B.1.2.0.0

Nessa atualização, eu refiz a tool, agora é somente um único executável. O funcionamento dessa tool é parecido com a tool de scenario do UHD, só que essa é para a versão de PS2.
<br> Os arquivos da versão anterior NÃO são compatíveis com essa versão;
