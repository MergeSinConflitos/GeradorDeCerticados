using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_TBCertificados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBSolicitacoesCertificado",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CursoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CaminhoZip = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataDeCriacao = table.Column<DateOnly>(type: "date", nullable: false),
                    DataDeConclusao = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBSolicitacoesCertificado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBCertificados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CursoId = table.Column<Guid>(type: "uuid", nullable: false),
                    SolicitacaoCertificadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeAluno = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataDeGeracao = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBCertificados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBCertificados_TBSolicitacoesCertificado_SolicitacaoCertifi~",
                        column: x => x.SolicitacaoCertificadoId,
                        principalTable: "TBSolicitacoesCertificado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBCertificados_CursoId",
                table: "TBCertificados",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBCertificados_SolicitacaoCertificadoId",
                table: "TBCertificados",
                column: "SolicitacaoCertificadoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBSolicitacoesCertificado_CursoId",
                table: "TBSolicitacoesCertificado",
                column: "CursoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBCertificados");

            migrationBuilder.DropTable(
                name: "TBSolicitacoesCertificado");
        }
    }
}
