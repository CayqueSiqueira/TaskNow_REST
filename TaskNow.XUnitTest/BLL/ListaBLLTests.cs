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
}
