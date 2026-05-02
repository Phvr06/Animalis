# Animalis - Plano Base do Projeto

## Visão Geral
**Animalis** é um roguelike 2D de sobrevivência com horda inspirado em *Vampire Survivors*, ambientado em um universo onde os animais da fazenda se rebelam contra seus opressores humanos.

O jogador escolhe entre quatro personagens jogáveis:

- **Raposa** - afinidade com **Fogo**
- **Porco** - afinidade com **Terra**
- **Peru** - afinidade com **Ar**
- **Ovelha** - afinidade com **Água**

Cada personagem possui uma arma inicial própria e uma identidade de combate distinta. Ao longo da run, o jogador enfrenta ondas progressivas de inimigos, coleta XP, sobe de nível e escolhe entre upgrades passivos, upgrades de armas já possuídas ou novas armas para compor a build.

O vertical slice será **single-player para PC**, com foco em provar o loop principal com:

- **3 mapas jogáveis** desbloqueados em sequência
- **4 personagens**
- **4 armas elementais iniciais**
- **3 armas universais**
- sistema de **até 3 armas ativas ao mesmo tempo**
- progressão dentro da run
- unlocks simples entre runs

---

## Escopo do Vertical Slice Expandido
A entrega planejada deve conter:

- 3 mapas jogáveis desbloqueados sequencialmente
- cada mapa tratado como arena de sobrevivência com estrutura infinita por chunks
- 4 personagens jogáveis
- 4 armas iniciais, uma para cada personagem
- 3 armas universais desbloqueáveis
- sistema de loadout com até 3 armas ativas
- sistema de afinidade elemental para armas fora do elemento do personagem
- sistema de XP, level up e escolha entre upgrades e novas armas
- hordas com dificuldade crescente baseada no tempo e no mapa
- pool base de inimigos comuns, elites e 1 pico final ou boss por mapa
- tela de derrota
- tela de vitória/clear de mapa
- unlocks simples entre runs
- desbloqueio sequencial de mapas

Fica fora do escopo inicial:

- multiplayer
- loja complexa
- crafting
- narrativa expandida
- meta progressão profunda com economia permanente
- árvore grande de habilidades
- mais de 3 mapas
- sistema de inventário amplo fora da run

---

## Core Loop
1. O jogador escolhe um personagem.
2. O jogador escolhe um mapa desbloqueado.
3. Inicia uma run com a arma inicial do personagem.
4. Derrota inimigos automaticamente com sua arma ativa.
5. Coleta XP dropado.
6. Sobe de nível e escolhe 1 entre 3 opções.
7. As opções podem incluir upgrades, evolução de armas atuais ou desbloqueio de nova arma.
8. O jogador monta uma build com até 3 armas ativas.
9. Sobrevive ao aumento de densidade, variedade de inimigos e eventos do mapa.
10. Enfrenta elites e um pico final de dificuldade ou boss.
11. Ganha a run ou morre.
12. Desbloqueia mapas, armas ou upgrades simples com base no desempenho.

---

## Estrutura dos Mapas

### Progressão entre mapas
- **Mapa 1** começa desbloqueado.
- **Mapa 2** é desbloqueado ao concluir o Mapa 1.
- **Mapa 3** é desbloqueado ao concluir o Mapa 2.
- Cada mapa deve ter identidade visual própria, pool de inimigos próprio ou combinação própria, além de um evento final diferente.

### Objetivo
Dar sensação de progressão entre runs sem abandonar a estrutura de sobrevivência por tempo.

### Regras
- O jogador deve escolher o mapa antes do início da run.
- Cada mapa deve ter seu próprio `StageDefinition` ou equivalente em dados.
- Cada mapa deve controlar:
  - tema visual
  - conjunto de chunks
  - regras de spawn
  - pacing
  - elite
  - evento final ou boss
  - recompensa de clear

---

## Estrutura Infinita dos Mapas
Cada mapa da run deve ser tratado como **infinito**.

### Objetivo
Dar sensação de deslocamento constante sem exigir construção manual de grandes áreas.

### Regra de implementação
- O jogador sempre pode continuar andando em qualquer direção.
- O cenário é montado por repetição de blocos ou chunks visuais.
- Obstáculos e decoração podem reaparecer em combinações simples.
- O gameplay não depende de chegar a um “fim” físico do mapa.
- A progressão da dificuldade é controlada pelo **tempo da run** e pelas regras do mapa, não pela posição do jogador.

### Diretrizes
- Os mapas devem ser visualmente abertos para facilitar leitura de hordas.
- Obstáculos devem ser poucos e claros.
- Cada mapa pode ter seu próprio conjunto de elementos visuais repetíveis.
- Pickups, inimigos e eventos devem spawnar em volta do jogador, respeitando distância mínima de segurança.

---

## Personagens e Armas Iniciais

### Raposa - Elemento Fogo
**Identidade:** agressividade, dano em área, queimadura, explosões.

**Arma inicial: Brasas de Rebelião**
- Dispara projéteis incendiários automáticos em direção aleatória próxima aos inimigos.
- Ao atingir, causa dano inicial e aplica queimadura.
- Inimigos queimando podem explodir ao morrer, atingindo outros próximos.

**Upgrades exclusivos sugeridos**
- **Combustão em Cadeia**: mortes de inimigos em chamas têm mais chance de explodir.
- **Pluma Ígnea**: projéteis deixam uma trilha breve de fogo no chão.

### Porco - Elemento Terra
**Identidade:** resistência, impacto, controle por área, combate mais pesado.

**Arma inicial: Tremor de Casco**
- Ondas de choque saem ao redor do personagem em intervalos regulares.
- Alcance curto a médio.
- Dano alto por acerto.
- Ideal para segurar grupos próximos.

**Upgrades exclusivos sugeridos**
- **Racha-Solo**: o ataque deixa fissuras que continuam causando dano por alguns segundos.
- **Muralha Brutal**: a onda de choque ganha empurrão extra e pequena chance de atordoar.

### Peru - Elemento Ar
**Identidade:** mobilidade, precisão, controle de espaço, ataques rápidos.

**Arma inicial: Lâminas de Vendaval**
- Projéteis cortantes de vento são lançados automaticamente na direção do inimigo mais próximo.
- Alta cadência, dano moderado.
- Chance de atravessar inimigos fracos.
- Boa arma para manter fluxo constante de dano.

**Upgrades exclusivos sugeridos**
- **Corrente Ascendente**: alguns disparos criam mini-redemoinhos que puxam inimigos levemente.
- **Rajada Caçadora**: aumenta alcance e permite que projéteis procurem alvos próximos após acertar.

### Ovelha - Elemento Água
**Identidade:** controle fluido, pressão constante, segurança e manipulação de status.

**Arma inicial: Orbes de Maré**
- Dispara automaticamente esferas de água em rajadas curtas contra alvos próximos.
- Dano moderado e trajetória estável.
- Aplica acúmulos de **Encharcado**.
- A arma base não congela inimigos; o congelamento entra apenas por upgrade futuro.

**Upgrades exclusivos sugeridos**
- **Correnteza Pesada**: inimigos Encharcados passam a sofrer lentidão crescente.
- **Maré Invernal**: ao atingir acúmulo máximo de Encharcado, a build passa a ter chance de congelar o alvo por pouco tempo.

---

## Sistema de Armas Ativas e Afinidade Elemental

### Regras do loadout
- Toda run começa com **1 arma ativa**, a arma inicial do personagem.
- O personagem pode ter **até 3 armas ativas** ao mesmo tempo.
- Novas armas podem aparecer como opções no level up enquanto houver slot disponível.
- Ao atingir 3 armas ativas, novas ofertas devem priorizar upgrades e passivos, não novas armas.
- O jogador nunca deve receber a mesma arma duas vezes como arma nova.

### Categorias de arma do slice
- **4 armas elementais iniciais**
  - Raposa/Fogo
  - Porco/Terra
  - Peru/Ar
  - Ovelha/Água
- **3 armas universais**
  - podem ser equipadas por qualquer personagem sem penalidade elemental

### Afinidade elemental
- A arma do próprio personagem funciona em **poder total**.
- Armas de outros elementos podem ser desbloqueadas e equipadas, mas funcionam com **penalidade de eficiência**.
- A penalidade deve ser configurável por dados e afetar pelo menos dano e efeitos secundários.
- Armas universais não sofrem penalidade elemental.

### Objetivo de design
- Permitir builds híbridas sem apagar a identidade do personagem.
- Recompensar o uso da fantasia elemental principal.
- Criar decisões relevantes no level up entre:
  - reforçar o que já funciona
  - abrir uma nova arma
  - buscar sinergia universal

---

## Estrutura de Upgrades

### Tipos de opção no level up
As opções da run devem ser divididas em quatro grupos:

#### 1. Upgrades universais
Afetam atributos gerais e podem aparecer para qualquer personagem.

Exemplos:
- +dano
- +velocidade de ataque
- +velocidade de movimento
- +área de efeito
- +vida máxima
- +pickup range
- +duração de efeitos
- +projétil extra
- +perfuração

#### 2. Upgrades de arma ativa
Melhoram diretamente uma arma já possuída pelo jogador.

Exemplos:
- mais projéteis
- menor cooldown
- maior alcance
- efeito secundário novo
- perfuração
- explosão no impacto
- efeito de controle adicional

#### 3. Novas armas
Liberam uma arma nova para ocupar um slot livre do loadout.

Regras:
- só podem aparecer se o jogador tiver menos de 3 armas ativas
- não podem repetir arma já possuída
- podem vir de:
  - armas elementais de outros personagens
  - armas universais desbloqueadas

#### 4. Upgrades elementais ou exclusivos
Reforçam a fantasia do personagem ou a sinergia de uma arma de elemento específico.

Exemplos:
- Ar: ricochete, sucção, velocidade de projétil
- Terra: escudo, stun, tremor residual
- Fogo: queimadura prolongada, explosão em cadeia
- Água: Encharcado mais forte, lentidão, congelamento por upgrade, ricochete líquido

### Regras de oferta
A cada level up, o jogador recebe **3 opções**.

Regras:
- Pelo menos 1 opção deve ser sempre útil para o estado atual da build.
- Se o jogador tiver menos de 3 armas ativas, uma das opções pode ser nova arma.
- Ao atingir 3 armas ativas, remover ofertas de arma nova.
- Upgrades de armas já equipadas devem ganhar prioridade.
- Upgrades exclusivos do personagem têm chance maior de aparecer no início da run.
- Upgrades universais estabilizam a progressão.
- Não oferecer upgrade já maximizado.
- Evitar 3 escolhas muito parecidas.

### Estrutura de raridade
- **Comum**: bônus diretos simples
- **Raro**: bônus fortes ou com sinergia
- **Épico**: modifica bastante a arma ou build
- **Lendário**: grande power spike, deve aparecer pouco

### Progressão dos upgrades
- upgrades comuns: até nível 3 ou 5
- upgrades exclusivos fortes: até nível 2 ou 3
- upgrades lendários: nível único

---

## Lógica de Unlocks Entre Runs

### Objetivo
Criar sensação de progresso sem desviar o foco do loop principal.

### O que pode ser desbloqueado
- mapas
- armas universais
- entrada de novas armas no pool de level up
- upgrades novos para entrar no pool
- personagens, se a equipe decidir começar com parte do elenco travado
- registros ou codex simples

### O que não deve existir no slice
- loja complexa
- árvore gigante de upgrades permanentes
- moeda com dezenas de usos
- grind excessivo

### Estrutura recomendada
Os unlocks devem ser baseados em **feitos claros**, não em farm longo.

Exemplos:
- concluir o mapa 1: desbloqueia o mapa 2
- concluir o mapa 2: desbloqueia o mapa 3
- derrotar 1 elite: desbloqueia 1 arma universal no pool
- sobreviver X minutos com cada personagem: desbloqueia upgrade exclusivo raro
- vencer o mapa 3: desbloqueia modo desafio simples ou conteúdo bônus do slice

### Regras
- unlocks precisam ser fáceis de entender
- cada unlock deve ter condição visível
- o jogador deve receber feedback imediato ao desbloquear algo
- unlocks devem ampliar variedade, não quebrar balanceamento

### Estrutura mínima de dados
Cada unlock deve ter:
- `id`
- `nome`
- `descrição`
- `condição`
- `tipo de recompensa`
- `conteúdo liberado`
- `status` bloqueado/desbloqueado

### Fluxo recomendado
1. A run termina.
2. O jogo avalia feitos do jogador.
3. Compara com a lista de unlocks ainda bloqueados.
4. Desbloqueia todos os critérios atendidos.
5. Exibe uma tela ou resumo de recompensas desbloqueadas.
6. Atualiza o pool de mapas, armas e upgrades disponíveis para runs futuras.

---

## Sistema de Build
A build de cada run surge da combinação de:
- personagem escolhido
- arma inicial
- até 2 armas adicionais desbloqueadas durante a run
- upgrades universais
- upgrades exclusivos
- afinidade elemental
- oferta aleatória controlada no level up

### Objetivo de design
Mesmo com apenas 4 armas elementais e 3 universais no slice, o jogador deve sentir diferença entre:
- build de dano bruto
- build de área
- build de controle
- build de sobrevivência
- build de status elemental
- build híbrida com penalidade off-element

---

## Inimigos e Escalada

### Curva sugerida por mapa
- **início**: poucos inimigos lentos
- **meio**: aumento de densidade e surgimento de variantes mais rápidas ou resistentes
- **pico**: grupos mistos, elite e pressão real
- **final**: boss ou evento final de sobrevivência intensa

### Diretriz
Os mapas posteriores devem:
- aumentar a pressão mais cedo
- introduzir combinações de inimigos mais difíceis
- justificar visualmente a mudança de estágio

---

## Arquitetura Recomendada

### Módulos
- **Bootstrap**: inicialização das cenas e ligação entre dados e runtime
- **Player**: movimento, vida, estado runtime e afinidade elemental
- **Loadout**: slots de arma ativa, aquisição de arma e upgrades por arma
- **Combat**: projéteis, dano, status e colisão
- **Enemies**: perseguição, spawn, elite e boss
- **Run**: tempo, XP, level up, ofertas e estado da run
- **Stage**: mapa, chunks, regras de estágio e progressão entre mapas
- **Meta**: unlocks e persistência simples
- **Data**: ScriptableObjects de personagens, armas, upgrades, inimigos, estágios e catálogo de conteúdo
- **UI**: HUD, menu, seleção de personagem, seleção de mapa, level up, derrota, vitória e unlocks

### Padrões
- `ScriptableObject` para dados autorados
- `ContentCatalog` para registrar personagens, armas e inimigos válidos do slice
- estado da run em classes runtime
- `Object Pool` para inimigos, projéteis e VFX
- eventos C# simples para level up, morte, unlock e fim de run
- evitar singleton excessivo e event bus global

---

## Divisão do Grupo

### Programador 1 - Player, loadout, combate e progressão do jogador
Responsável por tudo que nasce do lado do jogador.

Entregas:
- movimento do player
- vida e dano do player
- estado runtime do personagem
- sistema de armas automáticas
- loadout com até 3 armas ativas
- projéteis e hit detection
- XP e coleta de pickups
- level up
- aplicação de upgrades
- oferta de novas armas no level up
- sistema de afinidade elemental das armas
- efeitos de status causados pelo player
- seleção de personagem ligada aos dados do personagem

Arquivos ou sistemas sob responsabilidade:
- `Player`
- `Loadout`
- `Combat`
- parte de `Run` ligada a XP, level up e ofertas
- integração de `CharacterDefinition`, `WeaponDefinition`, `UpgradeDefinition`

### Programador 2 - Inimigos, mapas, run flow e meta progressão
Responsável por tudo que nasce do lado do mundo e do sistema de run.

Entregas:
- IA básica dos inimigos
- spawn director
- progressão temporal da run
- estrutura infinita por chunks para os 3 mapas
- regras de estágio por mapa
- elites e boss ou evento final
- controle de densidade e dificuldade
- tela de vitória e derrota
- sistema de unlocks entre runs
- persistência simples dos unlocks
- desbloqueio sequencial de mapas
- integração de `EnemyDefinition`, `StageDefinition`, `UnlockDefinition`

Arquivos ou sistemas sob responsabilidade:
- `Enemies`
- `Stage`
- parte de `Run` ligada a tempo e milestones
- `Meta/Unlocks`
- fluxo de começo e fim da run
- fluxo de escolha de mapa

### 2 Artistas/Designers - Conteúdo, UI e direção visual
Responsável por clareza visual, assets e definição de conteúdo.

Entregas:
- identidade visual dos 4 personagens com os elementos atualizados
- redefinição completa da Ovelha para Água
- definição visual e funcional das 3 armas universais
- HUD
- telas de menu, seleção de personagem, seleção de mapa, derrota e vitória
- layout de escolha de level up e armas
- VFX simples e feedback visual
- definição visual dos 3 mapas
- apoio no balanceamento e design de upgrades, armas e inimigos
- documentação visual e organização de assets

---

## Contrato Entre os 2 Programadores

### Programador 1 fornece
- `PlayerStatsRuntime` ou equivalente
- sistema de dano recebendo `DamageData`
- sistema de loadout com slots de arma
- sistema de aquisição de arma nova
- sistema de aplicação de upgrades
- resolução de afinidade elemental
- eventos de `OnLevelUp`, `OnWeaponUnlocked`, `OnPlayerDeath`

### Programador 2 fornece
- `EnemyDefinition`
- `StageDefinition`
- sistema de spawn com pontos ou áreas válidas
- fluxo de run com tempo atual e milestones
- sistema de unlock persistente
- evento de `OnMapUnlocked`, `OnRunEnded`

### Regras de integração
- dados autorados em `ScriptableObject`
- `ContentCatalog` registra o conteúdo oficial do slice
- runtime state em classes ou `MonoBehaviour`
- evitar editar o mesmo script pelos dois
- integrar por prefabs, eventos simples e estruturas de dados estáveis
- uma integração obrigatória no fim de cada marco de entrega

---

## Marcos de Entrega

## Semana 1 - Base jogável - Concluída
**Objetivo concluído:** ter o jogo rodando com loop mínimo e placeholders.

Resultado esperado já entregue:
- player andando
- inimigos surgindo
- arma automática funcionando
- XP sendo coletado
- mapa infinito básico
- derrota funcional

---

## Dia 1 - Fundação do escopo expandido
**Objetivo:** preparar o projeto para múltiplas armas, múltiplos mapas e dados escaláveis.

### Programador 1
- consolidar a estrutura de dados para personagens, armas e catálogo de conteúdo
- implementar a base do sistema de loadout com até 3 armas ativas
- adaptar o level up para suportar opções de upgrade ou nova arma
- integrar afinidade elemental básica para armas fora do elemento do personagem
- garantir que a seleção de personagem continue ligada aos dados

### Programador 2
- criar estrutura de `StageDefinition` para 3 mapas
- preparar fluxo de seleção de mapa e estado de desbloqueio
- adaptar o sistema de run para reconhecer o mapa atual
- separar regras de chunk e pacing por mapa
- preparar persistência simples para unlock de mapa

### Artistas/Designers
- atualizar a bíblia elemental dos personagens
- redefinir completamente a Ovelha como personagem de Água
- definir a fantasia e função das 3 armas universais
- definir tema visual dos 3 mapas
- desenhar wireframes de seleção de mapa e HUD com múltiplas armas

### Entregável do dia
- projeto preparado para múltiplas armas e múltiplos mapas
- estrutura de dados estável para continuar a produção
- direção de design atualizada e alinhada

---

## Dia 2 - Multi-arma e progressão inicial de mapa
**Objetivo:** transformar a run em uma experiência de build funcional e iniciar a progressão entre mapas.

### Programador 1
- implementar desbloqueio de novas armas via level up
- integrar as 4 armas elementais como opções secundárias off-element
- implementar as 3 armas universais
- impedir duplicação de arma no loadout
- conectar upgrades de arma ao sistema de loadout

### Programador 2
- implementar desbloqueio sequencial dos 3 mapas
- conectar clear de mapa a unlock do mapa seguinte
- preparar regras de spawn, elite e pacing específicas de cada mapa
- adaptar tela de vitória para identificar qual mapa foi concluído
- persistir progresso de mapas desbloqueados

### Artistas/Designers
- produzir ícones, nomes e leitura visual das 7 armas do slice
- montar placeholders funcionais dos 3 mapas
- ajustar HUD para exibir até 3 armas ativas
- apoiar definição visual do fluxo de unlocks entre mapas

### Entregável do dia
- player pode fechar o mapa 1 e desbloquear o mapa 2
- novas armas aparecem no level up
- sistema de 3 slots ativos está funcional

---

## Dia 3 - Identidade elemental e conteúdo jogável completo
**Objetivo:** consolidar a identidade dos personagens, dos mapas e da progressão do slice.

### Programador 1
- implementar upgrades exclusivos dos 4 personagens
- finalizar identidade da Raposa/Fogo e do Peru/Ar com as funções invertidas
- implementar kit da Ovelha/Água e sua progressão para slow ou freeze por upgrade
- ajustar penalidade de arma off-element
- melhorar a priorização das escolhas de level up

### Programador 2
- adicionar elites e evento final ou boss por mapa
- finalizar diferenças práticas entre os 3 mapas
- refinar curvas de dificuldade por mapa
- garantir spawn seguro e pacing consistente
- integrar unlocks extras de armas ou upgrades ao término de mapas

### Artistas/Designers
- refinar feedback visual de fogo, ar, terra e água
- criar apresentação própria para a Ovelha de Água
- diferenciar visualmente os 3 mapas
- melhorar telas de vitória, derrota e unlock

### Entregável do dia
- 4 personagens com identidade clara
- 3 mapas estruturalmente jogáveis
- Ovelha de Água completamente redefinida no slice

---

## Dia 4 - Fechamento, balanceamento e polish final
**Objetivo:** fechar o loop completo do mapa 1 ao mapa 3 e estabilizar a entrega final.

### Programador 1
- balancear sinergias entre armas elementais e universais
- revisar regras de oferta de novas armas
- impedir ofertas inúteis ou redundantes
- revisar dano, cooldown, área e efeitos por afinidade elemental
- estabilizar o sistema de 3 armas ativas
- corrigir bugs de combate, level up e afinidade elemental
- ajustar balanceamento final das armas
- revisar clareza de stats, slots e escolhas
- revisar edge cases de troca ou desbloqueio de armas

### Programador 2
- revisar unlocks e persistência completa entre runs
- ajustar dificuldade do mapa 1 ao mapa 3
- polir fluxos de seleção de mapa, fim de run e desbloqueio
- estabilizar bosses ou eventos finais
- corrigir bugs de mapa, estágio, spawn, boss, chunk e fluxo de run
- otimizar gargalos óbvios
- revisar pacing final dos 3 mapas

### Artistas/Designers
- finalizar UI de seleção de mapa e level up
- refinar apresentação dos slots de arma
- ajustar legibilidade visual das builds híbridas
- apoiar balanceamento com feedback visual e leitura de combate
- aplicar assets públicos finais prioritários
- produzir VFX próprios viáveis
- fechar apresentação visual da build

### Entregável do dia
- build estável
- loop completo sem bugs críticos
- progressão pelos 3 mapas funcionando
- apresentação pronta

---

## Checklist de Entrega Final
O vertical slice expandido está pronto quando:
- existe uma run completa com início, progressão e fim
- os 4 personagens estão jogáveis
- cada personagem tem arma e identidade distintas
- a Raposa é de Fogo, o Peru é de Ar e a Ovelha é de Água
- a Ovelha só ganha congelamento por upgrade, não na arma base
- existem 3 mapas desbloqueados em sequência
- o mapa infinito funciona nos 3 estágios sem travar o fluxo
- o jogador pode ter até 3 armas ativas
- o level up oferece upgrades e novas armas de forma coerente
- as armas off-element funcionam com penalidade configurável
- existem 3 armas universais utilizáveis por qualquer personagem
- há unlocks simples entre runs
- a build é estável do começo ao fim
- o jogo comunica bem dano, XP, morte, level up, arma nova, upgrade e unlock
