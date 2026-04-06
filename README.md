# KafkaCommerce: Ecossistema Event-Driven & Saga Coreografada

Criei este projeto como uma implementação de referência de um ecossistema de microserviços distribuídos, focado em resolver os desafios de consistência eventual e comunicação assíncrona em cenários de alta escala.

Utilizei o **Apache Kafka** como backbone de mensageria para orquestrar todo o fluxo de pedidos, gestão de stock e notificações em tempo real.

---

## Arquitetura do Sistema

A solução foi desenhada seguindo os princípios da **Clean Architecture** e **DDD (Domain-Driven Design)**, garantindo um código testável, desacoplado e de fácil manutenção.

### Componentes

- **API de Pedidos (Orders API)**  
  Porta de entrada (.NET 9) onde os pedidos são persistidos no MongoDB e o fluxo de eventos é iniciado.

- **Worker de Estoque (Stock Worker)**  
  Responsável por gerir o inventário com transações ACID num banco PostgreSQL.

- **Worker de Notificação**  
  Reage aos eventos finais para simular o envio de alertas ao cliente após a conclusão do processo.

- **Infrastructure**  
  Stack composta por Kafka (KRaft mode), MongoDB e PostgreSQL.

---

## Padrão Saga Coreografada

Para manter a integridade dos dados entre diferentes bases de dados (MongoDB e PostgreSQL) sem depender de transações distribuídas pesadas, foi utilizado o padrão **Saga Coreografada**.

### Fluxo do Processo

1. **API de Pedidos**
   - Cria o pedido com status `Processando`
   - Publica o evento `PedidoRealizadoEvent`

2. **Worker de Estoque**
   - Consome o evento
   - Valida o stock no PostgreSQL:
     - ✅ Se houver stock → publica `EstoqueConfirmadoEvent`
     - ❌ Se não houver → publica `EstoqueInsuficienteEvent`

3. **API de Pedidos**
   - Consome o feedback
   - Atualiza o status para:
     - `Aprovado` ou `Cancelado`

4. **Worker de Notificação**
   - Consome o resultado final
   - Notifica o utilizador sobre o sucesso ou falha da compra

---

## Tecnologias e Padrões

- **Runtime:** .NET 9 (C#)
- **Mensageria:** Apache Kafka
- **Bases de Dados:**
  - MongoDB (NoSQL para pedidos)
  - PostgreSQL (Relacional para estoque)
- **Contentorização:** Docker e Docker Compose

---

## Como Executar

### Pré-requisitos

- Docker e Docker Compose instalados
- SDK do .NET 9 (opcional, para execução fora do Docker)

---

### ▶️ Passo 1: Levantar a Infraestrutura

```bash
docker-compose up -d --build
```