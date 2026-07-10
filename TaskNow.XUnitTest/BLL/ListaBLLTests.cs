using Moq;
using TaskNow.BLL.Entities;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Lista;
using TaskNow.DTO.Utils;

namespace TaskNow.XUnitTest.BLL;

public class ListaBLLTests
{
    [Fact]
    public async Task CriarPadraoAsync_DeveFalhar_QuandoUsuarioNaoTemAcesso()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(false);

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.CriarPadraoAsync(quadroId);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Voce nao tem acesso a este quadro.", retorno.Mensagem);
        listaDalMock.Verify(x => x.CreateAsync(It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task CriarPadraoAsync_DeveCriarTodasAsListas_QuandoQuadroVazio()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listasIniciais = new List<ListaDTO>();
        var listasFinais = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "A fazer", Ordem = 1 },
            new() { Id = 2, QuadroId = quadroId, Nome = "Em andamento", Ordem = 2 },
            new() { Id = 3, QuadroId = quadroId, Nome = "Em teste", Ordem = 3 },
            new() { Id = 4, QuadroId = quadroId, Nome = "Concluido", Ordem = 4 }
        };

        listaDalMock.SetupSequence(x => x.ObterPorQuadroOrdenadoAsync(quadroId))
            .ReturnsAsync(listasIniciais)
            .ReturnsAsync(listasFinais);

        listaDalMock.Setup(x => x.ObterProximaOrdemAsync(quadroId)).ReturnsAsync(1);
        listaDalMock.Setup(x => x.CreateAsync(It.IsAny<ListaDTO>())).ReturnsAsync((ListaDTO dto) => dto);

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.CriarPadraoAsync(quadroId);

        // Assert
        Assert.True(retorno.Sucesso);
        Assert.NotNull(retorno.Dados);
        Assert.Equal(4, retorno.Dados.Count);
        Assert.Equal("Listas padrao processadas com sucesso.", retorno.Mensagem);

        listaDalMock.Verify(x => x.CreateAsync(It.Is<ListaDTO>(d => d.Nome == "A fazer" && d.Ordem == 1)), Times.Once);
        listaDalMock.Verify(x => x.CreateAsync(It.Is<ListaDTO>(d => d.Nome == "Em andamento" && d.Ordem == 2)), Times.Once);
        listaDalMock.Verify(x => x.CreateAsync(It.Is<ListaDTO>(d => d.Nome == "Em teste" && d.Ordem == 3)), Times.Once);
        listaDalMock.Verify(x => x.CreateAsync(It.Is<ListaDTO>(d => d.Nome == "Concluido" && d.Ordem == 4)), Times.Once);
    }

    [Fact]
    public async Task CriarPadraoAsync_DeveCriarApenasNaoExistentes_QuandoAlgumasJaExistem()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listasIniciais = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "A fazer", Ordem = 1 }
        };
        var listasFinais = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "A fazer", Ordem = 1 },
            new() { Id = 2, QuadroId = quadroId, Nome = "Em andamento", Ordem = 2 },
            new() { Id = 3, QuadroId = quadroId, Nome = "Em teste", Ordem = 3 },
            new() { Id = 4, QuadroId = quadroId, Nome = "Concluido", Ordem = 4 }
        };

        listaDalMock.SetupSequence(x => x.ObterPorQuadroOrdenadoAsync(quadroId))
            .ReturnsAsync(listasIniciais)
            .ReturnsAsync(listasFinais);

        listaDalMock.Setup(x => x.ObterProximaOrdemAsync(quadroId)).ReturnsAsync(2);
        listaDalMock.Setup(x => x.CreateAsync(It.IsAny<ListaDTO>())).ReturnsAsync((ListaDTO dto) => dto);

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.CriarPadraoAsync(quadroId);

        // Assert
        Assert.True(retorno.Sucesso);
        Assert.NotNull(retorno.Dados);
        Assert.Equal(4, retorno.Dados.Count);

        listaDalMock.Verify(x => x.CreateAsync(It.Is<ListaDTO>(d => d.Nome == "A fazer")), Times.Never);
        listaDalMock.Verify(x => x.CreateAsync(It.Is<ListaDTO>(d => d.Nome == "Em andamento" && d.Ordem == 2)), Times.Once);
        listaDalMock.Verify(x => x.CreateAsync(It.Is<ListaDTO>(d => d.Nome == "Em teste" && d.Ordem == 3)), Times.Once);
        listaDalMock.Verify(x => x.CreateAsync(It.Is<ListaDTO>(d => d.Nome == "Concluido" && d.Ordem == 4)), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoUsuarioNaoAutenticado()
    {
        // Arrange 
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;

        usuarioMock.SetupGet(x => x.UsuarioId).Returns(string.Empty);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = "Nova Lista"
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.CriarAsync(request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Usuario nao autenticado.", retorno.Mensagem);
        listaDalMock.Verify(x => x.CreateAsync(It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoNomeExceder80Caracteres()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = new string('a', 81)
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.CriarAsync(request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Nome da lista nao pode exceder 80 caracteres.", retorno.Mensagem);
        listaDalMock.Verify(x => x.CreateAsync(It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoNomeDuplicadoNoMesmoQuadro()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listasDoQuadro = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "A Fazer" }
        };

        listaDalMock.Setup(x => x.ObterPorQuadroOrdenadoAsync(quadroId)).ReturnsAsync(listasDoQuadro);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = "  a fazer  "
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.CriarAsync(request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Ja existe uma lista com este nome neste quadro.", retorno.Mensagem);
        listaDalMock.Verify(x => x.CreateAsync(It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveCriar_QuandoNomeNaoDuplicado()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listasDoQuadro = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "A Fazer" }
        };

        listaDalMock.Setup(x => x.ObterPorQuadroOrdenadoAsync(quadroId)).ReturnsAsync(listasDoQuadro);
        listaDalMock.Setup(x => x.ObterProximaOrdemAsync(quadroId)).ReturnsAsync(2);
        listaDalMock.Setup(x => x.CreateAsync(It.IsAny<ListaDTO>())).ReturnsAsync((ListaDTO dto) => dto);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = "Em Progresso"
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.CriarAsync(request);

        // Assert
        Assert.True(retorno.Sucesso);
        Assert.Equal("Lista criada com sucesso.", retorno.Mensagem);
        listaDalMock.Verify(x => x.CreateAsync(It.Is<ListaDTO>(d => d.Nome == "Em Progresso" && d.QuadroId == quadroId)), Times.Once);
    }

    [Fact]
    public async Task EditarAsync_DeveFalhar_QuandoNomeExceder80Caracteres()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int listaId = 10;
        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listaAtual = new ListaDTO { Id = listaId, QuadroId = quadroId, Nome = "Antigo" };
        listaDalMock.Setup(x => x.GetByIdAsync(listaId)).ReturnsAsync(listaAtual);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = new string('b', 81)
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.EditarAsync(listaId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Nome da lista nao pode exceder 80 caracteres.", retorno.Mensagem);
        listaDalMock.Verify(x => x.EditAsync(It.IsAny<int>(), It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task EditarAsync_DeveFalhar_QuandoNomeDuplicadoNoMesmoQuadro()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int listaId = 2;
        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listaAtual = new ListaDTO { Id = listaId, QuadroId = quadroId, Nome = "Lista Atual" };
        listaDalMock.Setup(x => x.GetByIdAsync(listaId)).ReturnsAsync(listaAtual);

        var listasDoQuadro = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "A Fazer" },
            listaAtual
        };

        listaDalMock.Setup(x => x.ObterPorQuadroOrdenadoAsync(quadroId)).ReturnsAsync(listasDoQuadro);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = "  a fazer  "
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.EditarAsync(listaId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Ja existe uma lista com este nome neste quadro.", retorno.Mensagem);
        listaDalMock.Verify(x => x.EditAsync(It.IsAny<int>(), It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task EditarAsync_DeveEditar_QuandoMantemMesmoNome()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int listaId = 2;
        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listaAtual = new ListaDTO { Id = listaId, QuadroId = quadroId, Nome = "A Fazer" };
        listaDalMock.Setup(x => x.GetByIdAsync(listaId)).ReturnsAsync(listaAtual);

        var listasDoQuadro = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "Outra Lista" },
            listaAtual
        };

        listaDalMock.Setup(x => x.ObterPorQuadroOrdenadoAsync(quadroId)).ReturnsAsync(listasDoQuadro);
        listaDalMock.Setup(x => x.EditAsync(listaId, It.IsAny<ListaDTO>())).ReturnsAsync((int id, ListaDTO dto) => dto);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = "A Fazer"
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.EditarAsync(listaId, request);

        // Assert
        Assert.True(retorno.Sucesso);
        Assert.Equal("Lista atualizada com sucesso.", retorno.Mensagem);
        listaDalMock.Verify(x => x.EditAsync(listaId, It.Is<ListaDTO>(d => d.Nome == "A Fazer")), Times.Once);
    }

    [Fact]
    public async Task ReordenarAsync_DeveFalhar_QuandoUsuarioNaoTemAcesso()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(false);

        var request = new ListaReordenarRequestDTO { ListaIdsOrdenados = [1, 2, 3] };
        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.ReordenarAsync(quadroId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Voce nao tem acesso a este quadro.", retorno.Mensagem);
        listaDalMock.Verify(x => x.AtualizarOrdensAsync(It.IsAny<Dictionary<int, int>>()), Times.Never);
    }

    [Fact]
    public async Task ReordenarAsync_DeveFalhar_QuandoRequisicaoVazia()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var request = new ListaReordenarRequestDTO { ListaIdsOrdenados = [] };
        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.ReordenarAsync(quadroId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("A lista de IDs enviados nao pode ser vazia.", retorno.Mensagem);
        listaDalMock.Verify(x => x.AtualizarOrdensAsync(It.IsAny<Dictionary<int, int>>()), Times.Never);
    }

    [Fact]
    public async Task ReordenarAsync_DeveFalhar_QuandoContemIdsDuplicados()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var request = new ListaReordenarRequestDTO { ListaIdsOrdenados = [1, 2, 2] };
        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.ReordenarAsync(quadroId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("A requisicao contem IDs duplicados.", retorno.Mensagem);
        listaDalMock.Verify(x => x.AtualizarOrdensAsync(It.IsAny<Dictionary<int, int>>()), Times.Never);
    }

    [Fact]
    public async Task ReordenarAsync_DeveFalhar_QuandoContemIdDeOutroQuadro()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listasDoQuadro = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "Lista 1" },
            new() { Id = 2, QuadroId = quadroId, Nome = "Lista 2" }
        };
        listaDalMock.Setup(x => x.ObterPorQuadroOrdenadoAsync(quadroId)).ReturnsAsync(listasDoQuadro);

        var request = new ListaReordenarRequestDTO { ListaIdsOrdenados = [1, 3] };
        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.ReordenarAsync(quadroId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Um ou mais IDs enviados nao pertencem a este quadro.", retorno.Mensagem);
        listaDalMock.Verify(x => x.AtualizarOrdensAsync(It.IsAny<Dictionary<int, int>>()), Times.Never);
    }

    [Fact]
    public async Task ReordenarAsync_DeveFalhar_QuandoFaltaIdDoQuadro()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listasDoQuadro = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "Lista 1" },
            new() { Id = 2, QuadroId = quadroId, Nome = "Lista 2" }
        };
        listaDalMock.Setup(x => x.ObterPorQuadroOrdenadoAsync(quadroId)).ReturnsAsync(listasDoQuadro);

        var request = new ListaReordenarRequestDTO { ListaIdsOrdenados = [1] };
        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.ReordenarAsync(quadroId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("A quantidade de IDs enviados nao corresponde ao total de listas do quadro.", retorno.Mensagem);
        listaDalMock.Verify(x => x.AtualizarOrdensAsync(It.IsAny<Dictionary<int, int>>()), Times.Never);
    }

    [Fact]
    public async Task ReordenarAsync_DeveReordenar_QuandoDadosValidos()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var listasDoQuadro = new List<ListaDTO>
        {
            new() { Id = 1, QuadroId = quadroId, Nome = "Lista 1", Ordem = 1 },
            new() { Id = 2, QuadroId = quadroId, Nome = "Lista 2", Ordem = 2 }
        };
        var listasReordenadas = new List<ListaDTO>
        {
            new() { Id = 2, QuadroId = quadroId, Nome = "Lista 2", Ordem = 1 },
            new() { Id = 1, QuadroId = quadroId, Nome = "Lista 1", Ordem = 2 }
        };

        listaDalMock.SetupSequence(x => x.ObterPorQuadroOrdenadoAsync(quadroId))
            .ReturnsAsync(listasDoQuadro)
            .ReturnsAsync(listasReordenadas);

        listaDalMock.Setup(x => x.AtualizarOrdensAsync(It.IsAny<Dictionary<int, int>>())).Returns(Task.CompletedTask);

        var request = new ListaReordenarRequestDTO { ListaIdsOrdenados = [2, 1] };
        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.ReordenarAsync(quadroId, request);

        // Assert
        Assert.True(retorno.Sucesso);
        Assert.Equal("Listas reordenadas com sucesso.", retorno.Mensagem);
        Assert.NotNull(retorno.Dados);
        Assert.Equal(2, retorno.Dados[0].Id);
        Assert.Equal(1, retorno.Dados[1].Id);

        listaDalMock.Verify(x => x.AtualizarOrdensAsync(It.Is<Dictionary<int, int>>(dict => 
            dict.Count == 2 && dict[2] == 1 && dict[1] == 2)), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoUsuarioSemAcessoAoQuadro()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(false);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = "Nova Lista"
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.CriarAsync(request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Voce nao tem acesso a este quadro.", retorno.Mensagem);
        listaDalMock.Verify(x => x.CreateAsync(It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoNomeVazio()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int quadroId = 1;
        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = "   "
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.CriarAsync(request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Nome da lista e obrigatorio.", retorno.Mensagem);
        listaDalMock.Verify(x => x.CreateAsync(It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task EditarAsync_DeveFalhar_QuandoListaNaoEncontrada()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int listaId = 1;
        listaDalMock.Setup(x => x.GetByIdAsync(listaId)).ReturnsAsync((ListaDTO?)null);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = 1,
            Nome = "Editada"
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.EditarAsync(listaId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Lista nao encontrada.", retorno.Mensagem);
        listaDalMock.Verify(x => x.EditAsync(It.IsAny<int>(), It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task EditarAsync_DeveFalhar_QuandoUsuarioSemAcessoAoQuadro()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int listaId = 1;
        int quadroId = 10;
        var listaAtual = new ListaDTO { Id = listaId, QuadroId = quadroId, Nome = "Original" };
        listaDalMock.Setup(x => x.GetByIdAsync(listaId)).ReturnsAsync(listaAtual);

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(false);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = "Editada"
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.EditarAsync(listaId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Voce nao tem acesso a este quadro.", retorno.Mensagem);
        listaDalMock.Verify(x => x.EditAsync(It.IsAny<int>(), It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task EditarAsync_DeveFalhar_QuandoNomeVazio()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int listaId = 1;
        int quadroId = 10;
        var listaAtual = new ListaDTO { Id = listaId, QuadroId = quadroId, Nome = "Original" };
        listaDalMock.Setup(x => x.GetByIdAsync(listaId)).ReturnsAsync(listaAtual);

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);

        var request = new ListaCriarRequestDTO
        {
            QuadroId = quadroId,
            Nome = ""
        };

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.EditarAsync(listaId, request);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Nome da lista e obrigatorio.", retorno.Mensagem);
        listaDalMock.Verify(x => x.EditAsync(It.IsAny<int>(), It.IsAny<ListaDTO>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_DeveFalhar_QuandoListaNaoEncontrada()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int listaId = 1;
        listaDalMock.Setup(x => x.GetByIdAsync(listaId)).ReturnsAsync((ListaDTO?)null);

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.ExcluirAsync(listaId);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Lista nao encontrada.", retorno.Mensagem);
        listaDalMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_DeveFalhar_QuandoUsuarioSemAcesso()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int listaId = 1;
        int quadroId = 10;
        var lista = new ListaDTO { Id = listaId, QuadroId = quadroId };
        listaDalMock.Setup(x => x.GetByIdAsync(listaId)).ReturnsAsync(lista);

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(false);

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.ExcluirAsync(listaId);

        // Assert
        Assert.False(retorno.Sucesso);
        Assert.Equal("Voce nao tem acesso a este quadro.", retorno.Mensagem);
        listaDalMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_DeveExcluirLista_QuandoValido()
    {
        // Arrange
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        int listaId = 1;
        int quadroId = 10;
        var lista = new ListaDTO { Id = listaId, QuadroId = quadroId };
        listaDalMock.Setup(x => x.GetByIdAsync(listaId)).ReturnsAsync(lista);

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(quadroId, "user-1")).ReturnsAsync(true);
        listaDalMock.Setup(x => x.DeleteAsync(listaId)).ReturnsAsync(true);

        var bll = new ListaBLL(listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        // Act
        var retorno = await bll.ExcluirAsync(listaId);

        // Assert
        Assert.True(retorno.Sucesso);
        Assert.Equal("Lista excluida com sucesso.", retorno.Mensagem);
        listaDalMock.Verify(x => x.DeleteAsync(listaId), Times.Once);
    }
}
