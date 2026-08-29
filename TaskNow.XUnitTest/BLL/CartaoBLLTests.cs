using Moq;
using TaskNow.BLL.Entities;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Cartao;
using Xunit;

namespace TaskNow.XUnitTest.BLL;

public class CartaoBLLTests
{
    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoUsuarioNaoAutenticado()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns(string.Empty);
        var request = new CartaoCriarRequestDTO { ListaId = 10, Titulo = "Teste" };
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.CriarAsync(request);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Usuario nao autenticado.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.CreateAsync(It.IsAny<CartaoDTO>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoListaNaoExiste()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var request = new CartaoCriarRequestDTO { ListaId = 10, Titulo = "Teste" };
        
        listaDalMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync((ListaDTO)null);
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.CriarAsync(request);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Lista nao encontrada.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.CreateAsync(It.IsAny<CartaoDTO>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoUsuarioSemAcessoAoQuadro()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var request = new CartaoCriarRequestDTO { ListaId = 10, Titulo = "Teste" };
        var listaFake = new ListaDTO { Id = 10, QuadroId = 1, Nome = "A Fazer" };
        
        listaDalMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(listaFake);
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(1, "user-1")).ReturnsAsync(false);
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.CriarAsync(request);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Voce nao tem acesso a este quadro.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.CreateAsync(It.IsAny<CartaoDTO>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoTituloVazio()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var request = new CartaoCriarRequestDTO { ListaId = 10, Titulo = "" };
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.CriarAsync(request);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Titulo do cartao e obrigatorio.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.CreateAsync(It.IsAny<CartaoDTO>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveCriarCartao_QuandoRequestValido()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var request = new CartaoCriarRequestDTO { ListaId = 10, Titulo = "Teste" };
        var listaFake = new ListaDTO { Id = 10, QuadroId = 1, Nome = "A Fazer" };
        var cartaoCriado = new CartaoDTO { Id = 100, ListaId = 10, Titulo = "Teste", Ordem = 1 };
        
        listaDalMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(listaFake);
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(1, "user-1")).ReturnsAsync(true);
        cartaoDalMock.Setup(x => x.ObterProximaOrdemAsync(10)).ReturnsAsync(1);
        cartaoDalMock.Setup(x => x.CreateAsync(It.IsAny<CartaoDTO>())).ReturnsAsync(cartaoCriado);
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.CriarAsync(request);

        Assert.True(retorno.Sucesso);
        Assert.Equal("Cartao criado com sucesso.", retorno.Mensagem);
        Assert.Equal(100, retorno.Dados?.Id);
        cartaoDalMock.Verify(x => x.CreateAsync(It.Is<CartaoDTO>(c => c.Titulo == "Teste" && c.Ordem == 1)), Times.Once);
    }

    [Fact]
    public async Task EditarAsync_DeveFalhar_QuandoCartaoNaoExiste()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var request = new CartaoEditarRequestDTO { Titulo = "Teste Editado" };
        
        cartaoDalMock.Setup(x => x.GetByIdAsync(100)).ReturnsAsync((CartaoDTO)null);
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.EditarAsync(100, request);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Cartao nao encontrado.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.EditAsync(It.IsAny<int>(), It.IsAny<CartaoDTO>()), Times.Never);
    }

    [Fact]
    public async Task EditarAsync_DeveFalhar_QuandoTituloVazio()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var request = new CartaoEditarRequestDTO { Titulo = "" };
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.EditarAsync(100, request);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Titulo do cartao e obrigatorio.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.EditAsync(It.IsAny<int>(), It.IsAny<CartaoDTO>()), Times.Never);
    }

    [Fact]
    public async Task EditarAsync_DeveFalhar_QuandoUsuarioSemAcesso()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var request = new CartaoEditarRequestDTO { Titulo = "Teste Editado" };
        var cartaoFake = new CartaoDTO { Id = 100, ListaId = 10, Titulo = "Teste Velho" };
        var listaFake = new ListaDTO { Id = 10, QuadroId = 1 };
        
        cartaoDalMock.Setup(x => x.GetByIdAsync(100)).ReturnsAsync(cartaoFake);
        listaDalMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(listaFake);
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(1, "user-1")).ReturnsAsync(false);
        
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.EditarAsync(100, request);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Voce nao tem acesso a este quadro.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.EditAsync(It.IsAny<int>(), It.IsAny<CartaoDTO>()), Times.Never);
    }

    [Fact]
    public async Task EditarAsync_DeveEditarCartao_QuandoRequestValido()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var request = new CartaoEditarRequestDTO { Titulo = "Teste Editado", Descricao = "Nova desc" };
        var cartaoFake = new CartaoDTO { Id = 100, ListaId = 10, Titulo = "Teste Velho" };
        var listaFake = new ListaDTO { Id = 10, QuadroId = 1 };
        var cartaoEditado = new CartaoDTO { Id = 100, ListaId = 10, Titulo = "Teste Editado", Descricao = "Nova desc" };
        
        cartaoDalMock.Setup(x => x.GetByIdAsync(100)).ReturnsAsync(cartaoFake);
        listaDalMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(listaFake);
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(1, "user-1")).ReturnsAsync(true);
        cartaoDalMock.Setup(x => x.EditAsync(100, It.IsAny<CartaoDTO>())).ReturnsAsync(cartaoEditado);
        
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.EditarAsync(100, request);

        Assert.True(retorno.Sucesso);
        Assert.Equal("Cartao atualizado com sucesso.", retorno.Mensagem);
        Assert.Equal("Teste Editado", retorno.Dados?.Titulo);
        cartaoDalMock.Verify(x => x.EditAsync(100, It.Is<CartaoDTO>(c => c.Titulo == "Teste Editado" && c.Descricao == "Nova desc")), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_DeveFalhar_QuandoCartaoNaoExiste()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        cartaoDalMock.Setup(x => x.GetByIdAsync(100)).ReturnsAsync((CartaoDTO)null);
        
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.ExcluirAsync(100);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Cartao nao encontrado.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_DeveFalhar_QuandoUsuarioSemAcesso()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var cartaoFake = new CartaoDTO { Id = 100, ListaId = 10 };
        var listaFake = new ListaDTO { Id = 10, QuadroId = 1 };
        
        cartaoDalMock.Setup(x => x.GetByIdAsync(100)).ReturnsAsync(cartaoFake);
        listaDalMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(listaFake);
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(1, "user-1")).ReturnsAsync(false);
        
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.ExcluirAsync(100);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Voce nao tem acesso a este quadro.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_DeveExcluirCartao_QuandoUsuarioTemAcesso()
    {
        var cartaoDalMock = new Mock<ICartaoDAL>();
        var listaDalMock = new Mock<IListaDAL>();
        var quadroDalMock = new Mock<IQuadroDAL>();
        var usuarioMock = new Mock<IUsuarioContexto>();

        usuarioMock.SetupGet(x => x.UsuarioId).Returns("user-1");
        var cartaoFake = new CartaoDTO { Id = 100, ListaId = 10 };
        var listaFake = new ListaDTO { Id = 10, QuadroId = 1 };
        
        cartaoDalMock.Setup(x => x.GetByIdAsync(100)).ReturnsAsync(cartaoFake);
        listaDalMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(listaFake);
        quadroDalMock.Setup(x => x.UsuarioTemAcessoAsync(1, "user-1")).ReturnsAsync(true);
        cartaoDalMock.Setup(x => x.DeleteAsync(100)).ReturnsAsync(true);
        
        var bll = new CartaoBLL(cartaoDalMock.Object, listaDalMock.Object, quadroDalMock.Object, usuarioMock.Object);

        var retorno = await bll.ExcluirAsync(100);

        Assert.True(retorno.Sucesso);
        Assert.Equal("Cartao excluido com sucesso.", retorno.Mensagem);
        cartaoDalMock.Verify(x => x.DeleteAsync(100), Times.Once);
    }
}
