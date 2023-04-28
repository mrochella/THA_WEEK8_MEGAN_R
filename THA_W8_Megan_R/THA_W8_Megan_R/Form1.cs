using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace THA_W8_Megan_R
{
    public partial class Form1 : Form
    {
        string connectionString = "server = localhost;uid = root;pwd = Meganmegan2009;database=premier_league";
        string sqlQuery;
        MySqlConnection sqlConnect;
        MySqlCommand sqlCommand;
        MySqlDataAdapter sqlAdapter;
        DataTable dtPlayer = new DataTable();
        DataTable dtTeam = new DataTable();
        DataTable dtPilih = new DataTable();
        DataTable dtLagi = new DataTable();
        DataTable dtHome = new DataTable();
        DataTable dtAway = new DataTable();
        DataTable dtLainnya = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.MintCream;
            sqlConnect = new MySqlConnection(connectionString);
        }

        private void playerDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtPlayer.Clear();
            dtTeam.Clear();
            dtPilih.Clear();

            sqlQuery = "SELECT team_name,team_id from team;";
            sqlCommand = new MySqlCommand(sqlQuery,sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeam);

            comboBox_team.DataSource = dtTeam;
            comboBox_team.ValueMember = "team_id";
            comboBox_team.DisplayMember = "team_name";
        }

        private void comboBox_team_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dtPlayer.Clear();
            dtPilih.Clear();

            sqlQuery = "SELECT p.player_name FROM player p INNER JOIN team t ON p.team_id = t.team_id AND t.team_id = '" + comboBox_team.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPilih);

            comboBox_player.DataSource = dtPilih;
            comboBox_player.ValueMember = "player_name";
            comboBox_player.DisplayMember = "player_name";
        }

        private void comboBox_player_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dtPlayer.Clear();
            dtLagi.Clear();

            sqlQuery = "SELECT p.player_name,t.team_name,n.nation,if(p.playing_pos = 'F','Forward',if(p.playing_pos = 'M','Midfielder',if (p.playing_pos = 'D','Defender','Goalkeeper'))),p.team_number FROM player p, team t , nationality n WHERE p.team_id = t.team_id AND n.nationality_id = p.nationality_id AND p.player_name = '" + comboBox_player.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPlayer);

            label_whatpName.Text = dtPlayer.Rows[0][0].ToString(); //player name
            label_whattName.Text = dtPlayer.Rows[0][1].ToString(); //team name
            label_whatPos.Text = dtPlayer.Rows[0][2].ToString(); //nationality
            label_whatsNum.Text = dtPlayer.Rows[0][3].ToString(); //position
            label_whatyCard.Text = dtPlayer.Rows[0][4].ToString(); //squad number

            sqlQuery = "SELECT ifnull(sum(case when d.`type`='CY'then 1 else 0 end),0),ifnull(sum(case when d.`type`='CR'then 1 else 0 end),0),ifnull(sum(case when d.`type`='GO'then 1 else 0 end),0),ifnull(sum(case when d.`type`='GW'then 1 else 0 end),0),ifnull(sum(case when d.`type`='GP'then 1 else 0 end),0),ifnull(sum(case when d.`type`='PM'then 1 else 0 end),0) FROM dmatch d , player p WHERE d.player_id = p.player_id AND p.player_name = '" + comboBox_player.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtLagi);

            label_whatrCard.Text = dtLagi.Rows[0][0].ToString(); //yellow card
            label_whatGoal.Text = dtLagi.Rows[0][1].ToString(); //red card
            label_whatownGoal.Text = dtLagi.Rows[0][2].ToString(); //goal
            label_whatgPenalty.Text = dtLagi.Rows[0][3].ToString(); //own goal
            label_whatpMissed.Text = dtLagi.Rows[0][4].ToString(); //goal penalty
            label_whatNat.Text = dtLagi.Rows[0][5].ToString(); //penalty missed
        }

        private void showMatchDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtTeam.Clear();
            dtPilih.Clear();
            dtAway.Clear();
            dtHome.Clear();

            sqlQuery = "SELECT team_name,team_id from team;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeam);

            comboBox_teamlagi.DataSource = dtTeam;
            comboBox_teamlagi.ValueMember = "team_id";
            comboBox_teamlagi.DisplayMember = "team_name";
        }

        private void comboBox_teamlagi_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dtAway.Clear();
            dtHome.Clear();
            dtPilih.Clear();

            sqlQuery = "SELECT d.match_id from dmatch d, `match` m , `match` n WHERE d.match_id = m.match_id AND (m.team_home = d.team_id or m.team_away = d.team_id) AND d.team_id = '" + comboBox_teamlagi.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPilih);

            comboBox_pilih.DataSource = dtPilih;
            comboBox_pilih.ValueMember = "match_id";
            comboBox_pilih.DisplayMember = "match_id";
        }

        private void comboBox_pilih_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dtAway.Clear();
            dtHome.Clear();
            dtLainnya.Clear();

            label_awayTeam.Text = "Away Team";
            label_homeTeam.Text = "Home Team";
            label_Details.Text = "Match Details";

            sqlQuery = "SELECT p.player_name,t.team_name,if(p.playing_pos = 'F','Forward',if(p.playing_pos = 'M','Midfielder',if(p.playing_pos = 'D','Defender','Goalkeeper'))) position FROM player p, team t , `match` m WHERE t.team_id = m.team_home AND t.team_id = p.team_id AND m.match_id = '" + comboBox_pilih.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtHome);
            dataGridView_home.DataSource = dtHome;

            sqlQuery = "SELECT p.player_name,t.team_name,if(p.playing_pos = 'F','Forward',if(p.playing_pos = 'M','Midfielder',if(p.playing_pos = 'D','Defender','Goalkeeper'))) position FROM player p, team t , `match` m WHERE t.team_id = m.team_away AND t.team_id = p.team_id AND m.match_id = '" + comboBox_pilih.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtAway);
            dataGridView_away.DataSource = dtAway;

            sqlQuery = "SELECT d.`minute`,p.player_name,t.team_name,if(d.`type`='CY','Yellow Card',if(d.`type`='CR','Red Card',if(d.`type`='GO','Goal',if(d.`type`='GW','Own Goal',if(d.`type`='GP','Goal Penalty','Penalty Missed'))))) type FROM player p, team t , `match` m, dmatch d WHERE t.team_id = d.team_id AND d.player_id = p.player_id AND d.match_id = '" + comboBox_pilih.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtLainnya);
            dataGridView_lainnya.DataSource = dtLainnya;
        }
    }
}
