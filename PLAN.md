# Animalis - Plano Base do Projeto

  ## Visão Geral
  **Animalis** é um roguelike 2D de sobrevivência com horda inspirado em *Vampire Survivors*, ambientado em um universo onde os animais da fazenda se rebelam contra seus opressores humanos. O jogador
  escolhe entre quatro personagens jogáveis:

  - **Raposa** - afinidade com **Ar**
  - **Porco** - afinidade com **Terra**
  - **Peru** - afinidade com **Fogo**
  - **Ovelha** - afinidade com **Gelo**

  Cada personagem possui uma arma inicial própria e uma identidade de combate distinta. Ao longo da run, o jogador enfrenta ondas progressivas de inimigos, coleta XP, sobe de nível e escolhe upgrades que
  fortalecem atributos, armas e efeitos elementais.

  O primeiro vertical slice será **single-player para PC**, com foco em provar o loop principal do jogo com **1 mapa jogável infinito**, **4 personagens**, progressão dentro da run e **unlocks simples
  entre runs**.

  ---

  ## Escopo do Vertical Slice
  A entrega do primeiro mês deve conter:

  - 1 mapa jogável com tema de fazenda
  - mapa **infinito** com geração/repetição contínua de chunks visuais
  - 4 personagens jogáveis
  - 4 armas iniciais, uma para cada personagem
  - sistema de XP, level up e escolha entre upgrades
  - hordas com dificuldade crescente baseada no tempo
  - 5 inimigos comuns
  - 1 elite
  - 1 evento final de pico de dificuldade ou boss
  - tela de derrota
  - tela de vitória ou sobrevivência até o tempo limite
  - unlocks simples entre runs

  Fica fora do escopo inicial:

  - multiplayer
  - loja complexa
  - crafting
  - narrativa expandida
  - múltiplos mapas completos
  - meta progressão profunda com economia permanente
  - árvore grande de habilidades

  ---

  ## Core Loop
  1. O jogador escolhe um personagem.
  2. Inicia uma run no mapa da fazenda.
  3. Derrota inimigos automaticamente com sua arma inicial.
  4. Coleta XP dropado.
  5. Sobe de nível e escolhe 1 entre 3 upgrades.
  6. Sobrevive ao aumento de densidade e variedade de inimigos.
  7. Enfrenta elites e um pico final de dificuldade.
  8. Ganha a run ou morre.
  9. Desbloqueia conteúdo simples com base no desempenho.

  ---

  ## Mapa Infinito
  O mapa da run deve ser tratado como **infinito**.

  ### Objetivo
  Dar sensação de deslocamento constante sem exigir construção de um mundo enorme manualmente.

  ### Regra de implementação
  - O jogador sempre pode continuar andando em qualquer direção.
  - O cenário é montado por repetição de blocos/chunks visuais.
  - Obstáculos e decoração podem reaparecer em combinações simples.
  - O gameplay não depende de chegar a um “fim” do mapa.
  - A progressão da dificuldade é controlada pelo **tempo da run**, não pela posição do jogador.

  ### Diretrizes
  - O mapa deve ser visualmente aberto para facilitar leitura de hordas.
  - Obstáculos devem ser poucos e claros.
  - O fundo pode repetir tiles, cercas, plantações, pedras e troncos de forma procedural simples.
  - Pickups, inimigos e eventos devem spawnar em volta do jogador, respeitando distância mínima de segurança.

  ---

  ## Personagens e Armas Iniciais

  ### Raposa - Elemento Ar
  **Identidade:** mobilidade, precisão, controle de espaço, ataques rápidos.

  **Arma inicial: Lâminas de Vendaval**
  - Projéteis cortantes de vento são lançados automaticamente na direção do inimigo mais próximo.
  - Alta cadência, dano moderado.
  - Chance de atravessar inimigos fracos.
  - Boa arma para manter fluxo constante de dano.

  **Upgrades exclusivos sugeridos**
  - **Corrente Ascendente**: alguns disparos criam mini-redemoinhos que puxam inimigos levemente.
  - **Rajada Caçadora**: aumenta alcance e permite que projéteis procurem alvos próximos após acertar.

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

  ### Peru - Elemento Fogo
  **Identidade:** agressividade, dano em área, queimadura, explosões.

  **Arma inicial: Brasas de Rebelião**
  - Dispara projéteis incendiários automáticos em direção aleatória próxima aos inimigos.
  - Ao atingir, causa dano inicial e aplica queimadura.
  - Inimigos queimando podem explodir ao morrer, atingindo outros próximos.

  **Upgrades exclusivos sugeridos**
  - **Combustão em Cadeia**: mortes de inimigos em chamas têm mais chance de explodir.
  - **Pluma Ígnea**: projéteis deixam uma trilha breve de fogo no chão.

  ### Ovelha - Elemento Gelo
  **Identidade:** controle, segurança, desaceleração, congelamento.

  **Arma inicial: Estilhaços de Geada**
  - Lança automaticamente cristais de gelo em rajadas curtas.
  - Dano moderado.
  - Aplica lentidão acumulativa.
  - Após acúmulo suficiente, congela o inimigo por pouco tempo.

  **Upgrades exclusivos sugeridos**
  - **Inverno Persistente**: inimigos congelados liberam uma nova onda de frio ao descongelar.
  - **Casulo de Gelo**: ao sofrer dano, cria uma explosão defensiva de gelo com cooldown.

  ---

  ## Estrutura de Upgrades

  ### Tipos de upgrade
  Os upgrades da run devem ser divididos em três grupos:

  #### 1. Upgrades universais
  Afetam atributos gerais e podem aparecer para qualquer personagem.

  Exemplos:
  - +dano
  - +velocidade de ataque
  - +velocidade de movimento
  - +área de efeito
  - +vida máxima
  - +regen ou cura ao subir de nível
  - +pickup range
  - +duração de efeitos
  - +projétil extra
  - +perfuração

  #### 2. Upgrades de arma
  Melhoram diretamente a arma inicial do personagem.

  Exemplos:
  - mais projéteis
  - menor cooldown
  - maior alcance
  - efeito secundário novo
  - perfuração
  - explosão no impacto
  - efeito de controle adicional

  #### 3. Upgrades elementais/exclusivos
  Reforçam a fantasia do elemento e ajudam a construir builds diferentes.

  Exemplos:
  - Ar: ricochete, sucção, velocidade de projétil
  - Terra: escudo, stun, tremor residual
  - Fogo: queimadura prolongada, explosão em cadeia
  - Gelo: lentidão mais forte, congelamento mais rápido, estilhaços secundários

  ### Regras de oferta de upgrades
  A cada level up, o jogador recebe **3 opções**.

  Regras:
  - Pelo menos 1 opção deve ser sempre útil para o estado atual da build.
  - Upgrades exclusivos do personagem têm chance maior de aparecer no início da run.
  - Upgrades universais estabilizam a progressão.
  - Upgrades de raridade maior aparecem mais tarde.
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

  Exemplo:
  **Queimadura Aprimorada**
  - Nível 1: aumenta duração da queimadura
  - Nível 2: aumenta dano por segundo
  - Nível 3: inimigos queimando espalham fogo ao morrer

  ---

  ## Lógica de Unlocks Entre Runs

  ### Objetivo
  Criar sensação de progresso sem desviar o foco do loop principal.

  ### O que pode ser desbloqueado
  - personagens
  - upgrades novos para entrar no pool
  - armas variantes futuras
  - registros/codex simples
  - modificadores leves de run

  ### O que não deve existir no slice
  - loja complexa
  - árvore gigante de upgrades permanentes
  - moeda com dezenas de usos
  - grind excessivo

  ### Estrutura recomendada
  Os unlocks devem ser baseados em **feitos claros**, não em farm longo.

  Exemplos:
  - sobreviver 3 minutos: desbloqueia novo upgrade universal
  - derrotar 1 elite: desbloqueia novo upgrade elemental
  - chegar ao minuto 8 com Raposa: desbloqueia variante de upgrade de Ar
  - vencer uma run: desbloqueia dificuldade seguinte ou modo desafio simples
  - jogar com todos os 4 personagens: desbloqueia um upgrade raro compartilhado

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
  5. Exibe uma tela/resumo de recompensas desbloqueadas.
  6. Atualiza o pool de conteúdo disponível para runs futuras.

  ---

  ## Sistema de Build
  A build de cada run surge da combinação de:
  - personagem escolhido
  - arma inicial
  - upgrades universais
  - upgrades exclusivos
  - oferta aleatória controlada no level up

  ### Objetivo de design
  Mesmo com apenas 1 arma inicial por personagem, o jogador deve sentir diferença entre:
  - build de dano bruto
  - build de área
  - build de controle
  - build de sobrevivência
  - build de status elemental

  ---

  ## Inimigos e Escalada

  ### Curva sugerida
  - **0-2 min**: poucos inimigos lentos
  - **2-5 min**: aumento de densidade e surgimento de variante mais rápida
  - **5-8 min**: grupos mistos e pressão real
  - **8-10/12 min**: elite e aumento forte de densidade
  - **final**: boss ou evento final de sobrevivência intensa

  ### Tipos iniciais
  - inimigo fraco corpo a corpo
  - inimigo rápido e frágil
  - inimigo resistente
  - elite com mais vida e comportamento simples diferenciado

  ---

  ## Arquitetura Recomendada

  ### Módulos
  - **Bootstrap**: inicialização das cenas e dados
  - **Player**: movimento, vida, stats, arma ativa
  - **Combat**: projéteis, dano, status e colisão
  - **Enemies**: perseguição, spawn, elite, boss
  - **Run**: tempo, XP, level up, estado da run
  - **Data**: ScriptableObjects de personagens, armas, upgrades, inimigos e estágio
  - **UI**: HUD, menu, seleção, derrota, vitória, unlocks

  ### Padrões
  - `ScriptableObject` para dados autorados
  - estado da run em classes runtime
  - `Object Pool` para inimigos, projéteis e VFX
  - eventos C# simples para level up, morte e fim de run
  - evitar singleton excessivo e event bus global

  ---

  ## Divisão do Grupo

  A divisão precisa reduzir conflito entre os dois programadores. Cada um deve ser dono de subsistemas diferentes, com interfaces simples entre eles.

  ### Programador 1 - Player, combate e progressão do jogador
  Responsável por tudo que nasce do lado do jogador.

  Entregas:
  - movimento do player
  - vida/dano do player
  - stats runtime do personagem
  - sistema de armas automáticas
  - projéteis e hit detection
  - XP e coleta de pickups
  - level up
  - aplicação de upgrades
  - efeitos de status causados pelo player
  - seleção de personagem ligada aos dados do personagem

  Arquivos/sistemas sob responsabilidade:
  - `Player`
  - `Combat`
  - parte de `Run` ligada a XP/level
  - integração de `CharacterDefinition`, `WeaponDefinition`, `UpgradeDefinition`

  ### Programador 2 - Inimigos, run flow, mapa e meta progressão
  Responsável por tudo que nasce do lado do mundo e do sistema de run.

  Entregas:
  - IA básica dos inimigos
  - spawn director
  - progressão temporal da run
  - mapa infinito e chunks
  - elites e boss/evento final
  - controle de densidade e dificuldade
  - tela de vitória/derrota
  - sistema de unlocks entre runs
  - persistência simples dos unlocks
  - integração de `EnemyDefinition`, `StageDefinition`, `UnlockDefinition`

  Arquivos/sistemas sob responsabilidade:
  - `Enemies`
  - `Stage/Map`
  - parte de `Run` ligada a tempo e waves
  - sistema de `Unlocks`
  - fluxo de começo/fim da run

  ### 2 Artistas/Designers - Conteúdo, UI e direção visual
  Responsável por clareza visual, assets e definição de conteúdo.

  Entregas:
  - identidade visual dos 4 personagens
  - placeholders e substituição gradual por assets públicos
  - HUD
  - telas de menu, seleção de personagem, derrota e vitória
  - VFX simples e feedback visual
  - definição visual do mapa da fazenda
  - apoio no balanceamento e design de upgrades/inimigos
  - documentação visual e organização de assets

  ---

  ## Contrato Entre os 2 Programadores

  Para evitar retrabalho, as integrações devem ser fechadas cedo.

  ### Programador 1 fornece
  - `PlayerStatsRuntime`
  - sistema de dano recebendo `DamageData`
  - interface de arma automática
  - sistema de aplicação de upgrades
  - eventos de `OnLevelUp`, `OnPlayerDeath`

  ### Programador 2 fornece
  - `EnemyDefinition`
  - sistema de spawn com pontos/áreas válidas
  - fluxo de run com tempo atual e milestones
  - sistema de unlock persistente
  - evento de `OnRunEnded`

  ### Regras de integração
  - dados autorados em `ScriptableObject`
  - runtime state em classes/MonoBehaviours
  - evitar editar o mesmo script pelos dois
  - integrar por prefabs, eventos simples e estruturas de dados estáveis
  - uma integração obrigatória no fim de cada semana

  ---

  ## Milestones Semanais

  ## Semana 1 - Base jogável
  **Objetivo:** ter o jogo rodando com loop mínimo e placeholders.

  ### Programador 1
  - implementar movimento do player
  - implementar vida/dano do player
  - criar sistema base de arma automática
  - criar primeiro projétil funcional
  - fazer XP drop e coleta
  - deixar 1 personagem totalmente jogável com placeholder

  ### Programador 2
  - implementar inimigo básico perseguidor
  - implementar spawn simples por tempo
  - criar loop inicial da run
  - montar base do mapa infinito com repetição de chunk
  - fazer derrota básica ao morrer

  ### Artistas/Designers
  - definir direção visual provisória
  - criar placeholders consistentes para player, inimigos, pickups e cenário
  - montar HUD mínima
  - organizar referência visual dos 4 personagens

  ### Meta da semana
  - build com player andando
  - inimigos surgindo
  - arma automática funcionando
  - XP sendo coletado
  - mapa infinito básico
  - derrota funcional

  ---

  ## Semana 2 - Progressão e conteúdo mínimo
  **Objetivo:** transformar o protótipo em uma run real.

  ### Programador 1
  - implementar sistema de level up com 3 escolhas
  - implementar upgrades universais
  - implementar as 4 armas iniciais
  - criar stats runtime por personagem
  - conectar seleção de personagem à gameplay

  ### Programador 2
  - adicionar 5 tipos de inimigos
  - implementar escalada temporal da run
  - adicionar elite
  - melhorar sistema de spawn
  - criar fluxo de vitória/evento final temporário

  ### Artista/Designer
  - montar tela de seleção de personagem
  - melhorar HUD de vida, XP e level up
  - criar ícones/nomes de upgrades temporários
  - iniciar substituição de placeholders prioritários

  ### Meta da semana
  - run completa do início ao fim
  - 4 personagens jogáveis
  - 5 tipos de inimigos
  - level up com escolha
  - elite funcional

  ---

  ## Semana 3 - Identidade e sistema de unlocks
  **Objetivo:** dar profundidade suficiente para o jogo parecer um slice e não só um protótipo técnico.

  ### Programador 1
  - implementar upgrades exclusivos por personagem
  - melhorar comportamento das armas
  - adicionar efeitos elementais
  - ajustar dano, cooldown, área e sinergias
  - corrigir problemas de progressão da build

  ### Programador 2
  - implementar unlocks entre runs
  - persistir unlocks localmente
  - refinar mapa infinito e spawn seguro
  - implementar boss ou pico final definitivo
  - ajustar curva de dificuldade

  ### Artista/Designer
  - refinar aparência dos personagens
  - melhorar feedback visual de hit, level up e morte
  - montar tela de unlocks/recompensas
  - escolher assets públicos definitivos que entrarão no slice

  ### Meta da semana
  - unlocks funcionais
  - upgrades exclusivos ativos
  - boss/pico final presente
  - identidade elemental perceptível nos 4 personagens

  ---

  ## Semana 4 - Polish e fechamento
  **Objetivo:** estabilizar a build e preparar entrega/apresentação.

  ### Programador 1
  - corrigir bugs de combate e upgrades
  - ajustar balanceamento das armas
  - melhorar consistência das colisões e dano
  - revisar clareza de stats e escolhas

  ### Programador 2
  - corrigir bugs de spawn, mapa e fluxo de run
  - estabilizar unlocks e telas de fim
  - revisar dificuldade e pacing
  - otimizar gargalos óbvios

  ### Artista/Designer
  - finalizar UI
  - aplicar assets públicos finais prioritários
  - produzir VFX próprios viáveis
  - fechar apresentação visual da build

  ### Meta da semana
  - build estável
  - loop completo sem bugs críticos
  - identidade visual minimamente coesa
  - apresentação pronta

  ---

  ## Checklist de Entrega Final
  O vertical slice está pronto quando:
  - existe uma run completa com início, progressão e fim
  - os 4 personagens estão jogáveis
  - cada personagem tem arma e identidade distintas
  - o mapa infinito funciona sem travar o fluxo
  - há level up com escolhas relevantes
  - há unlocks simples entre runs
  - a build é estável do começo ao fim
  - o jogo comunica bem dano, XP, morte, level up e upgrade