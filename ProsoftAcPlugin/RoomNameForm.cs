using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProsoftAcPlugin
{
    public partial class RoomNameForm : Form
    {
        public RoomNameForm()
        {
            InitializeComponent();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
        
        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
               

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void pub_marriagerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_marriagerm_opt.Text;
            close_btn.Enabled = true;
        }

        private void atriunrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = atriunrm_opt.Text;
            close_btn.Enabled = true;
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            Plugin.bARoom = true;
            this.Close();
        }

        private void bedrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = bedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void Mbedrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = Mbedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void chbedrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = chbedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void dinnigkitrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = dinnigkitrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void livingkitrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = livingkitrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void livdinrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = livdinrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void studyrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = studyrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void guestrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = guestrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void com_toilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = com_toilrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void attachedtoil_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = attachedtoil_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void servanrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = servanrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void veranrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = veranrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void tvrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = tvrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void drawrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = drawrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void dressrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = dressrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void multipurrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = multipurrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void passagerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = passagerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void loungerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = loungerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void workrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = workrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void livingrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = livingrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void diningrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = diningrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void kitrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = kitrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void puja_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = puja_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void stre_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = stre_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void bathrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = bathrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void wcrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = wcrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void washrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = washrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void toilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = toilrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void combtoilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = combtoilrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void kitchenetterm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = kitchenetterm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void familyrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = familyrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void utilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = utilrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void hallrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = hallrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void entrancerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = entrancerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void foyerrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = foyerrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void sitoutrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = sitoutrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void balcony_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = balcony_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void rmrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = rmrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pantryrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pantryrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void cabinrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = cabinrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void officerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = officerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void bakerrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = bakerrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void receptionrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = receptionrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void restaurantrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = restaurantrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void cafeteriarm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = cafeteriarm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void shworm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = shworm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void hotelrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = hotelrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void departmentalrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = departmentalrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void conferancehalrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = conferancehalrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void entrancelobbyrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = entrancelobbyrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void firectrlrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = firectrlrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void waitrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = waitrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void laundryrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = laundryrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void shoprm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = shoprm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void bankrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = bankrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void saferm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = saferm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_rmrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_rmrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_auditrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_auditrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_genralwrdrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_genralwrdrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_specialrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_specialrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_cinemarm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_cinemarm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_assem_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_assem_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_entrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_entrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_operthetrerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_operthetrerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_clinicrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_clinicrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_consultrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_consultrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_communityrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_communityrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_meetrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_meetrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_librm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_librm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_labrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_labrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_sevbath_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_sevbath_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void pub_servtoilrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = pub_servtoilrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void clsrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = clsrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void hostelrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = hostelrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void staffrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = staffrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void kgardenrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = kgardenrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void wrkshp_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = wrkshp_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void storagerm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = storagerm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void openshedrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = shedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void shedrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = shedrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void factoryrm_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = factoryrm_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void godown_opt_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = godown_opt.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void hotel_opt1_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = hotel_opt1.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }

        private void hotel_opt2_CheckedChanged(object sender, EventArgs e)
        {
            Commands.tmproomName = hotel_opt2.Text;
            close_btn.Enabled = true;
            Commands.brmnamechanged = true;
        }
    }
}
