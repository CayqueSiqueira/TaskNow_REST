using Moq;
using TaskNow.BLL.Entities;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Quadro;

namespace TaskNow.XUnitTest.BLL;

public class QuadroBLLTests
{
    [Fact]
    public async Task CriarAsync_DeveFalhar_QuandoNomeVazio()
    {
        var dal = new Mock<IQuadroDAL>();
        var usuario = new Mock<IUsuarioContexto>();
        usuario.SetupGet(x => x.UsuarioId).Returns("user-1");
        var bll = new QuadroBLL(dal.Object, usuario.Object);

        var retorno = await bll.CriarAsync(new QuadroCriarRequestDTO { Nome = "" });

        Assert.False(retorno.Sucesso);
        Assert.Equal("Nome do quadro e obrigatorio.", retorno.Mensagem);
    }

    [Fact]
    public async Task CriarAsync_DeveCriarQuadro_QuandoRequestValido()
    {
        var dal = new Mock<IQuadroDAL>();
        var usuario = new Mock<IUsuarioContexto>();
        usuario.SetupGet(x => x.UsuarioId).Returns("user-1");
        dal.Setup(x => x.CreateAsync(It.IsAny<QuadroDTO>()))
            .ReturnsAsync((QuadroDTO dto) =>
            {
                dto.Id = 1;
                return dto;
            });

        var bll = new QuadroBLL(dal.Object, usuario.Object);

        var retorno = await bll.CriarAsync(new QuadroCriarRequestDTO { Nome = "Projeto" });

        Assert.True(retorno.Sucesso);
        Assert.Equal(1, retorno.Dados?.Id);
        Assert.Equal("user-1", retorno.Dados?.DonoId);
    }
}