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
        var request = new CartaoCriarRequestDTO
        {
            ListaId = 10,
            Titulo = "Testar regras"
        };
 
        var bll = new CartaoBLL(
            cartaoDalMock.Object,
            listaDalMock.Object,
            quadroDalMock.Object,
            usuarioMock.Object
        );

        var retorno = await bll.CriarAsync(request);

        Assert.False(retorno.Sucesso);
        Assert.Equal("Usuario nao autenticado.", retorno.Mensagem);

        cartaoDalMock.Verify(x => x.CreateAsync(It.IsAny<CartaoDTO>()), Times.Never);
    }
}
