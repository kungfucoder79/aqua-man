﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    /// <summary>
    /// Class for simulating results from the I2C water sensor, usefull when debuging on a machine with out gpio pins
    /// </summary>
    public class EmptyI2CController : BaseI2CController, IAquaI2CController
    {
        #region Members
        private double[] _testVals = { -.133851981163, -.131133925915, -.082319402695, -.120988404751, -.099586254358, -.11855045557, -.112692189217, -.114612007141, -.112482345104, -.112688446045, -.119569540024, -.111382162571, -.109350764751, -.113313353062, -.104923987389, -.126730763912, -.121184623241, -.116140222549, -.081220650673, -.112520253658, -.117628192902, -.105109810829, -.118255650997, -.095262283087, -.102647316456, -.112291419506, -.125906121731, -.118316113949, -.112548232079, -.11099858284, -.098436814547, -.155741083622, -.111242055893, -.131249666214, -.116120243073, -.144087386131, -.114989316463, -.080632728338, -.113634645939, -.109135890007, -.107118058205, -.077586334944, -.086643582582, -.088034701347, -.094385111332, -.060449558496, -.091326981783, -.140480494499, -.100570082664, -.103776693344, -.100702702999, -.110061049461, -.118214547634, -.094841563702, -.103563439846, -.151062476635, -.140955400467, -.104241085052, -.123106408119, -.137078869343, -.111807799339, -.138054513931, -.108480763435, -.097783571482, -.120272469521, -.103390824795, -.103881621361, -.111757910252, -.097863864899, -.107592642307, -.101024210453, -.052324831486, -.091483467817, -.10614310503, -.105960929394, -.113338923454, -.098902332783, -.100424969196, -.11666353941, -.09034666419, -.115717220306, -.118448114395, -.098567247391, -.08349545598, -.102632915974, -.124636805058, -.101349067688, -.111279284954, -.127105271816, -.147617566586, -.10331941843, -.104924046993, -.158790528774, -.096006649733, -.153153383732, -.123773503304, -.103627288342, -.108239066601, -.085963654518, -.093007034063, -.085541385412, -.082505619526, -.092356961966, -.093811416626, -.098679894209, -.087703639269, -.083898752928, -.097503691912, -.073677092791, -.092475861311, -.068067568541, -.064383339882, -.065201175213, -.077033227682, -.074806123972, -.070783370733, -.072805196047, -.051639044285, -.049195945263, -.067839968204, -.066248756647, -.049882885814, -.023753646016, -.067507529259, -.031819814444, -.053384941816, -.078549414873, -.039639297128, .003773881495, -.015310421586, -.008052962273, .098065042496, .017733734846, -.011531671882, .024212254584, .021294997633, .06346257329, .043541488051, .039388495684, -.005985704064, .045239946246, .019590139389, .025254887342, .031309700012, .005965854973, .055272042751, .019602772593, .014149081707, -.000282169646, .010678382963, -.01636928767, -.010214863718, .020520794392, .061811292171, .024438163638, .040977704525, .037549516559, .076767802238, .076762348413, .070196014643, .200585246086, .183406877518, .177198374271, .131100308895, .163231766224, .161099553108, .163858127594, .130671179295, .191550219059, .141683590412, .140949177742, .154735684395, .113300561905, .109810078144, .115160799026, .165186703205, .157322394848, .137649655342, .15128633976, .166685497761, .15443829298, .127645075321, .134259366989, .128236544132, .119793832302, .219239544868, .180244100094, .165589022636, .16370934248, .193024754524, .129641747475, .169629192352, .173061168194, .175562775135, .165114915371, .184587574005, .181940150261, .156543791294, .156665062904, .201507902145, .197327768803, .192233014107, .19350874424, .181599318981, .222981953621, .199304425716, .207953500748, .248349714279, .206614017487, .196300053596, .215579938889, .198664999008, .213052296638, .267210268974, .214356613159, .242430686951, .226736998558, .22078461647, .245092272758, .250883936882, .26253695488, .24717874527, .232462096214, .276486968994, .274458169937, .26201403141, .250146341324, .280768346786, .277271318436, .265131855011, .269466614723, .294010806084, .248810052872, .262623667717, .283074164391, .257311892509, .30039563179, .32182135582, .335305380821, .325302553177, .368154811859, .360324001312, .353418636322, .295868563652, .318000292778, .332132673264, .320417404175, .320880055428, .345598387718, .328515577316, .363578081131, .359296345711, .345492911339, .348014783859, .368088293076, .346039223671, .333386540413, .357197785378, .381013274193, .341003775597, .363200306892, .341975045204, .373783159256, .344675278664, .362413907051, .380067038536, .351620435715, .396602988243, .431561899185, .439533424377, .401992511749, .378563070297, .348887991905, .400021123886, .400294351578, .437473154068, .419025087357, .425458908081, .432954978943, .422233963013, .42870516777, .430350923538, .431203794479, .420031070709, .384885835648, .397224164009, .416279697418, .409479475021, .445084285736, .419623374939, .442515802383, .470116710663, .43204870224, .464255046844, .423697662354, .461794042587, .479939746857, .488568162918, .456520605087, .491114044189, .489159822464, .513815069199, .497281217575, .509473752975, .499019384384, .477155780792, .508412504196, .53807311058, .51172618866, .539228391647, .513151025772, .526754331589, .584053277969, .509086894989, .521648216248, .532818365097, .50569601059, .492031764984, .541022825241, .555425405502, .585798454285, .556754684448, .580556917191, .57678694725, .606237649918, .584899616241, .562946891785, .554316043854, .589692163467, .596074581146, .589352273941, .587288284302, .627402114868, .584412193298, .561087369919, .650460529327, .620760917664, .610288190842, .632199525833, .62930188179, .612728881836, .619977140427, .606390810013, .607578134537, .618624544144, .637127161026, .626980257034, .651663160324, .614097166061, .624075937271, .626085090637, .650638484955, .652550172806, .639723968506, .672623682022, .632243824005, .632243824005, .652994537354, .676292657852, .670603895187, .699876070023, .673591041565, .667135953903, .656268453598, .713257741928, .712907838821, .712763357162, .704510211945, .69690580368, .682936525345, .697976922989, .721450138092, .721072435379, .698550796509, .683481168747, .74680557251, .7320520401, .667841005325, .668276834488, .677971601486, .721278572083, .734099578857, .656162786484, .718714666367, .714284038544, .746331357956, .755563640594, .742689990997, .839875507355, .780352449417, .842469596863, .801454925537, .815162658691, .78137960434, .839543628693, .812209129333, .795599842072, .809432506561, .784997987747, .811344718933, .833727836609, .782735347748, .794370365143, .818975162506, .810434436798, .850318908691, .80830078125, .812876987457, .83104801178, .860845565796, .882547664642, .892987632751, .891472148895, .843039417267, .831440448761, .860140323639, .896051502228, .865606498718, .8812748909, .830006408691, .856929588318, .853800296783, .880559444427, .918191051483, .866352748871, .83181180954, .86706495285, .885204315186, .892733383179, .891206550598, .86750202179, .860027885437, .914911651611, .924403095245, .912998008728, .931088066101, .972229671478, .923520469666, .909091567993, .962194252014, 1.001243305206, .959752845764, .932670402527, .915917301178, .922180843353, .973639583588, .944281101227, .975319194794, 1.030718898773, 1.080705547333, .965449237823, 1.02555809021, .978713989258, .971834182739, .95368680954, 1.027725028992, .999183940887, 1.018064117432, 1.008802890778, .996016025543, 1.029891586304, 1.005049991608, .987576580048, .997880935669, .982779121399, .954254436493, 1.020517444611, .979633617401, 1.013442420959, 1.064297389984, 1.004760074615, 1.025428199768, 1.028091049194, .971085548401, .995403671265, 1.034452819824, 1.019732189178, 1.093469333649, 1.068864440918, 1.038682937622, .978470802307, 1.023176860809, 1.023268890381, 1.064737510681, .995831871033, 1.027676582336, 1.037938213348, 1.006704044342, 1.006375312805, .999327087402, 1.079345417023, 1.084567928314, 1.032919025421, 1.059964752197, .974715900421, 1.042838668823, 1.04346036911, 1.034878730774, 1.114309692383, 1.051242160797, 1.145415019989, 1.112655735016, 1.161334609985, 1.064611911774, 1.0400806427, 1.12983083725, 1.107239246368, 1.136832046509, 1.078465557098, 1.104214668274, 1.014062690735, 1.111418628693, 1.12025384903, 1.145609760284, 1.239242935181, 1.175495147705, 1.140466403961, 1.093381881714, 1.154142665863, 1.131796360016, 1.183291912079, 1.193165493011, 1.116865634918, 1.171718025208, 1.129895782471, 1.17868013382, 1.147609901428, 1.158588695526, 1.069970703125, 1.122181987762, 1.158276939392, 1.123405456543, 1.108710670471, 1.178762149811, 1.160127925873, 1.181169700623, 1.054989242554, 1.161186695099, 1.15324382782, 1.152861309052, 1.13765335083, 1.199408149719, 1.17887544632, 1.282403469086, 1.091867160797, 1.253583145142, 1.181068134308, 1.213930892944, 1.202823162079, 1.124475765228, 1.292539596558, 1.109747409821, 1.324608707428, 1.266929721832, 1.296490192413, 1.25345249176, 1.19298992157, 1.204389858246, 1.219364547729, 1.213742542267, 1.289341640472, 1.291707897186, 1.276802062988, 1.29324388504, 1.366231155396, 1.29777803421, 1.326665496826, 1.312223434448, 1.242714214325, 1.293430995941, 1.247322463989, 1.283030605316, 1.287704181671, 1.336761474609, 1.347727966309, 1.33739938736, 1.354444980621, 1.292570114136, 1.306220054626, 1.301462078094, 1.299328041077, 1.311573410034, 1.259628391266, 1.371059036255, 1.328734207153, 1.344808673859, 1.303390979767, 1.334536361694, 1.375752353668, 1.439880847931, 1.422220516205, 1.341584777832, 1.376004981995, 1.36156539917, 1.319985675812, 1.370873260498, 1.410407161713, 1.405514431, 1.404833602905, 1.389714431763, 1.355436420441, 1.480582714081, 1.40607919693, 1.389310741425, 1.431102466583, 1.404951667786, 1.271459674835, 1.356855964661, 1.393672561646, 1.399429988861, 1.362463855743, 1.349478054047, 1.387286281586, 1.467648887634, 1.34038476944, 1.492406463623, 1.415285491943, 1.452414512634, 1.445678520203, 1.501524448395, 1.525434970856, 1.480157566071, 1.414341449738, 1.421507835388, 1.484213161469, 1.449830245972, 1.565709114075, 1.459148693085, 1.466344165802, 1.394736671448, 1.400492572784, 1.436984157562, 1.402999973297, 1.404450798035, 1.502105903625, 1.489684486389, 1.431553649902, 1.613342666626, 1.461007881165, 1.491668128967, 1.462127304077, 1.458059310913, 1.482638835907, 1.553341388702, 1.528091812134, 1.559354019165, 1.524429416656, 1.558787345886, 1.436398887634, 1.554616641998, 1.486302852631, 1.486879730225, 1.546256923676, 1.501810741425, 1.564603996277, 1.547381305695, 1.561528301239, 1.496387577057, 1.54684381485, 1.495565509796, 1.591512966156, 1.620231437683, 1.569581127167, 1.712459182739, 1.801677131653, 1.566680335999, 1.633054351807, 1.55094537735, 1.645811080933, 1.581969070435, 1.524414634705, 1.657917976379, 1.609750556946, 1.562691307068, 1.593084907532, 1.58288192749, 1.587768554688, 1.570964336395, 1.674451446533, 1.601831626892, 1.658488845825, 1.623713684082, 1.719082260132, 1.69316444397, 1.697209358215, 1.575478458405, 1.743285560608, 1.706426239014, 1.696041297913, 1.706187057495, 1.758827972412, 1.593935108185, 1.662334060669, 1.594392967224, 1.60620098114, 1.628016662598, 1.656996917725, 1.649468803406, 1.661480331421, 1.664414024353, 1.618595504761, 1.700668144226, 1.628569412231, 1.617329216003, 1.656999588013, 1.71554813385, 1.632427406311, 1.674237823486, 1.668379974365, 1.622876358032, 1.676560401917, 1.637419128418, 1.679005241394, 1.618153572083, 1.702057266235, 1.594238471985, 1.725506401062, 1.629745674133, 1.706734657288, 1.695276260376, 1.635545730591, 1.756843757629, 1.730568885803, 1.759766197205, 1.714472770691, 1.71445980072, 1.764524841309, 1.701131820679, 1.784596252441, 1.794493675232, 1.778272628784, 1.825988197327, 1.763391494751, 1.820025444031, 1.800560760498, 1.756135368347, 1.818066596985, 1.713487815857, 1.74338760376, 1.79756603241, 1.835170555115, 1.710629844666, 1.79204158783, 1.827982330322, 1.853924179077, 1.802738761902, 1.742203521729, 1.849266242981, 1.840660858154, 1.84965133667, 1.788930511475, 1.779655647278, 1.837682151794, 1.692215919495, 1.932386207581, 1.776985359192, 1.868470001221, 1.809354782104, 1.808838653564, 1.818439865112, 1.835584449768, 1.816273117065, 1.791474533081, 1.860296821594, 1.853010368347, 1.820127105713, 1.811409568787, 1.81108379364, 1.862674331665, 1.874911117554, 2.058234024048, 1.833759880066, 1.807842826843, 1.806296348572, 1.668245697021, 2.008410263062, 1.864589500427, 1.881772041321, 1.866958999634, 1.825196647644, 1.840063095093, 1.996708106995, 1.833358192444, 1.729088020325, 1.864574813843, 1.841853713989, 1.961580467224, 1.861169242859, 1.828486442566, 1.911022377014, 1.902821159363, 1.910611343384, 1.839583015442, 1.859508323669, 1.886576461792, 1.86996421814, 1.808218765259, 1.953046798706, 1.882117080688, 1.91739730835, 1.875959014893, 1.89214553833, 1.905428504944, 1.838241195679, 1.867242240906, 1.956218528748, 1.908864974976, 1.965205764771, 1.912711524963, 1.929970359802, 1.925888252258, 1.968782234192, 1.868628311157, 1.837270355225, 1.970482444763, 1.876693344116, 1.948044395447, 1.882606315613, 1.882606315613, 1.956474304199, 1.927202796936, 1.992058372498, 2.049368286133, 1.92160320282, 1.984218215942, 2.012881088257, 1.96157989502, 1.92938117981, 2.167521095276, 2.050903320313, 2.024683761597, 1.976435089111, 1.997225189209, 1.996476554871, 2.1233543396, 2.016990661621, 2.036487007141, 2.141207122803, 2.019936561584, 2.016103172302, 1.940670204163, 2.106123542786, 2.052653312683, 2.104708862305, 2.142609786987, 2.09359703064, 2.044570159912, 2.028888893127, 2.15337562561, 2.155987930298, 2.2001039505, 2.132955932617, 2.202368736267, 2.123637771606, 2.132360076904, 2.238979911804, 2.009040260315, 2.260580062866, 2.309559249878, 2.268144798279, 2.301274871826, 2.263045501709, 2.223784828186, 2.250575637817, 2.356553649902, 2.261110496521, 2.390511512756, 2.370222473145, 2.32418346405, 2.269501876831, 2.214481544495, 2.293059158325, 2.31233959198, 2.227751350403, 2.207363510132, 2.246328353882, 2.288626670837, 2.282607650757, 2.325451850891, 2.300419807434, 2.351642036438, 2.27380065918, 2.343104171753, 2.285097122192, 2.283854293823, 2.436734580994, 2.405554199219, 2.33547668457, 2.433163261414, 2.388182449341, 2.444428062439, 2.440801239014, 2.435752296448, 2.425500297546, 2.713877105713, 2.423947906494, 2.449465751648, 2.411002922058, 2.450830078125, 2.474275588989, 2.618012619019, 2.60751914978, 2.454042053223, 2.440155982971, 2.633688545227, 2.56183795929, 2.68423614502, 2.620939826965, 2.605628395081, 2.652174568176, 2.669924736023, 2.615515327454, 2.721573638916, 2.581678390503, 2.650066757202, 2.727529716492, 2.608360099792, 2.686866378784, 2.652836227417, 2.596510696411, 2.6725025177, 2.638737487793, 2.664820289612, 2.6036277771, 2.758117675781, 2.738857841492, 2.757565307617, 2.719689559937, 2.708839225769, 2.828925132751, 3.015060043335, 2.826155853271, 2.794000434875, 2.798633956909, 2.8944190979, 2.8452917099, 2.832853126526, 2.730518722534, 2.770744514465, 2.691190910339, 2.756754493713, 2.776202011108, 2.809873390198, 2.767449760437, 2.738025283813, 2.990041542053, 2.815937614441, 2.832975006104, 2.701258277893, 2.924226570129, 2.91110496521, 2.851624488831, 2.829370307922, 2.910606765747, 2.832166290283, 2.912165260315, 2.912488365173, 2.903072738647, 2.969537734985, 3.008107185364, 3.032893180847, 3.074006271362, 2.830235099792, 3.147637557983, 2.807471466064, 2.995035743713, 2.861347389221, 2.866677284241, 2.846675682068, 2.792630195618, 2.992535972595, 2.971872520447, 2.996300125122, 2.856863975525, 2.885627174377, 2.947770690918, 2.978715705872, 3.000257301331, 3.043292999268, 2.970250511169, 3.051068115234, 3.097379875183, 3.111069488525, 2.997144317627, 2.983040809631, 2.965021896362, 2.9559923172, 2.708046340942, 3.09309463501, 3.093040275574, 2.920010757446, 2.922661781311, 2.946403312683, 2.730752563477, 2.733684539795, 2.903921890259, 3.083302116394, 2.863444519043, 3.029783439636, 3.099268531799, 3.110744476318, 2.963416290283, 3.064027023315, 3.130445480347, 2.888722610474, 3.058123970032, 3.041801452637, 2.826047134399, 3.069506645203, 3.047093200684, 3.202085876465, 2.950584030151, 3.018250465393, 2.981200790405, 3.096064758301, 3.069397544861, 3.005278587341, 2.966031265259, 3.070428848267, 3.046775627136, 3.333864974976, 2.989379119873, 2.99288520813, 3.34808921814, 3.066039848328, 3.118652534485, 2.818237113953, 3.059214401245, 3.164872932434, 3.048149490356, 3.152104759216, 3.155431556702, 3.039760971069, 3.275150680542, 3.154925918579, 3.098235702515, 3.058331108093, 3.043832588196, 3.088840675354, 3.072444725037, 3.053632545471, 3.051587104797, 3.066519355774, 2.934066390991, 2.742593002319, 3.063714408875, 3.053464698792, 3.125971221924, 3.254294204712, 3.022108840942, 3.24760093689, 2.974686813354, 3.025943183899, 2.98600025177, 3.120791625977, 2.978427886963, 3.095330047607, 3.025495147705, 2.908363723755, 3.170749664307, 3.263618850708, 3.058025932312, 3.06579246521, 3.138726425171, 3.135533332825, 2.968107795715, 3.170569992065, 3.156470298767, 2.987796592712, 3.152016830444, 3.185785865784, 3.060385894775, 3.134666633606, 2.986784934998, 3.175261497498, 3.060694122314, 2.858038139343, 3.122198104858, 3.131195449829, 3.000336074829, 3.037794685364, 3.04327545166, 3.04327545166, 3.064743423462, 3.0719165802, 3.115992546082, 3.100551223755, 2.953128433228, 3.091417503357, 3.07574596405, 2.878314399719, 3.196314048767, 3.065188980103, 3.461122894287, 3.279899597168, 3.115454864502, 3.498725128174, 3.055181694031, 3.140892982483, 3.070995140076, 3.168441963196, 2.886508750916, 2.955650520325, 3.025642776489, 3.100204658508, 3.06177520752, 3.018496322632, 3.157046699524, 3.222959899902, 3.035094261169, 3.053785514832, 3.068668937683, 3.075308418274, 2.954941940308, 3.075289535522, 3.086304283142, 3.055110740662, 3.094606590271, 3.093494796753, 2.827868080139, 3.261494445801, 3.049975395203, 3.022162437439, 3.13593082428, 3.09353351593, 3.015194320679, 2.831692504883, 3.077705383301, 3.130598831177, 3.119853591919, 3.080626487732, 2.966229438782, 3.103995132446, 3.097253608704, 3.162842941284, 3.068690681458, 3.086581802368, 3.072720336914, 3.016299057007, 2.97310333252, 3.062064361572, 3.136194801331, 3.177452087402, 3.004503059387, 3.09981918335, 3.059880828857, 2.937589645386, 2.998418426514, 2.833737945557, 3.028167152405, 3.047824287415, 3.069189071655, 3.038544082642, 2.918639564514, 2.988935661316, 3.011723899841, 3.071257400513, 3.222793960571, 3.151025772095, 2.959892463684, 3.041060066223, 3.100620079041, 3.087703323364, 3.039692497253, 3.104452323914, 3.048995399475, 2.972766685486, 3.1050365448, 3.179367637634, 3.074053192139, 2.949571037292, 3.038373947144, 3.010837554932, 2.795280075073, 3.080170631409, 2.972918701172, 3.076380729675, 3.059282112122, 2.925553131104, 3.175222015381, 3.15168800354, 3.060626602173, 3.033794021606, 3.041299819946, 3.081807327271, 3.066673469543, 3.137210083008, 3.028235626221, 3.105640983582, 2.791387176514, 2.925457763672, 3.037173461914, 3.257106781006, 3.006737136841, 3.090825462341, 3.190595817566, 3.230738449097, 2.976134681702, 3.18651714325, 2.925184249878, 3.129533576965, 3.113687133789, 3.10364780426, 3.231351470947, 3.067017364502, 2.953934288025, 3.038641166687, 3.01465587616, 3.045025444031, 3.151514816284, 2.926720809937, 3.086865234375, 3.016942977905, 3.124994277954, 3.126898956299, 3.18607711792, 3.070900726318, 3.097068023682, 3.092942237854, 3.10964679718, 2.963730049133, 3.214455413818, 3.06413860321, 3.219188690186, 3.171775054932, 3.09676399231, 3.146268463135, 3.127090263367, 3.118834686279, 3.144352912903, 3.047964859009, 2.952018165588, 2.981079483032, 3.109964942932, 3.092037773132, 3.111448860168, 3.238697433472, 3.17310256958, 3.067805862427, 3.065267181396, 3.032911109924, 3.034966087341, 3.120548439026, 3.063488388062, 2.837267875671, 3.113062667847, 3.06451587677, 3.18564453125, 3.071503257751, 3.31962928772, 3.113864135742, 2.933094596863, 2.976991462708, 3.074218559265, 3.388762664795, 3.092503166199, 2.96427116394, 3.037164306641, 3.103387069702, 3.177534675598, 2.997849845886, 3.252304840088, 3.05735168457, 3.121872901917, 3.097488975525, 3.009736251831, 3.107076454163, 3.116165924072, 3.117572402954, 3.147391891479, 3.031043052673, 3.146946334839, 3.161409759521, 3.010178565979, 3.081843566895, 3.049284362793, 3.386781692505, 3.223476791382, 2.971332740784, 3.2356590271, 3.063093757629, 3.181497383118, 3.060454177856, 3.110461425781, 3.011462593079, 3.021173477173, 3.126028251648, 3.087105941772, 3.130026245117, 3.080504989624, 3.233381652832, 3.189678764343, 3.179128646851, 3.117922592163, 3.086367797852, 3.140666389465, 3.101391220093, 3.124568557739, 3.072652816772, 3.07794342041, 3.150092506409, 3.046034622192, 3.083575057983, 3.138263130188, 3.101104545593, 3.051431465149, 3.034509468079, 3.067845153809, 3.106800079346, 3.110415267944, 3.007608985901, 3.069418716431, 3.069113349915, 3.138828849792, 3.079663467407, 3.077460098267, 3.110143661499, 2.965487289429, 2.98516330719, 3.148525238037, 3.080456352234, 3.092296409607, 3.034470939636, 3.171681404114, 3.187088775635, 3.105086708069, 3.199580192566, 3.111180114746, 3.020065498352, 3.140438461304, 3.098666191101, 3.068312072754, 3.044766616821, 3.007000541687, 3.127620887756, 3.244342422485, 3.067537307739, 2.966128730774, 3.093571853638, 2.915069961548, 3.216439819336, 3.155773735046, 3.059954071045, 2.936128997803, 3.10484828949, 3.104121589661, 2.98550491333, 3.128576278687, 3.242328262329, 3.07895488739, 2.974365234375, 3.142351722717, 3.12144317627, 3.083607292175, 3.137571144104, 3.11722984314, 2.972760391235, 3.165766525269, 3.113001060486, 3.155919075012, 3.202657318115, 3.272061920166, 3.118649864197, 3.159145736694, 3.092815208435, 2.960667610168, 2.887592506409, 3.174966430664, 3.14617805481, 3.11311340332, 3.043969154358, 3.047173881531, 3.191120529175, 3.263089370728, 3.114195632935, 3.207391357422, 3.101257324219, 3.409350204468, 3.209534072876, 3.060860443115, 3.263027572632, 3.12705783844, 3.01074848175, 3.250661087036, 3.251824569702, 3.139817619324, 3.243772888184, 3.197686004639, 3.324963760376, 3.223830795288, 3.154427146912, 3.277233505249, 3.367358779907, 3.364948654175, 3.409686279297, 3.408913040161, 3.374157714844, 3.52087059021, 3.396478652954, 3.306329727173, 3.701662063599, 3.369646835327, 3.402099609375, 3.326622390747, 3.44388885498, 3.509110641479, 3.482560348511, 3.259550094604, 3.582042312622, 3.365861129761, 3.485018157959, 3.631065750122, 3.607445907593, 3.553522109985, 3.580162811279, 3.658857727051, 3.850023269653, 3.718280410767, 3.676881790161, 3.747543334961, 3.529779052734, 3.663580322266, 3.589663314819, 3.749978256226, 3.535432815552, 3.634351348877, 3.821271896362, 3.657690811157, 3.728554153442, 3.793915557861, 3.669969177246, 3.882331466675, 3.818589782715, 3.65120010376, 3.572058105469, 3.872853851318, 3.746685791016, 3.777532196045, 3.949127960205, 3.790328979492, 3.916533279419, 3.863370132446, 3.842975997925, 3.788369369507, 3.974742126465, 3.819982147217, 3.853731918335, 4.221220779419, 4.004071044922, 3.928249359131, 3.792335891724, 4.198194885254, 4.215401077271, 4.069239807129, 4.09372215271, 4.031641387939, 4.078859329224, 4.106832122803, 4.126189041138, 4.128179168701, 4.100196838379, 4.138779830933, 4.205228042603, 4.256855392456, 4.239992904663, 4.083721542358, 4.206217193604, 4.330503082275, 4.30039100647, 4.276781082153, 4.380331420898, 4.256148529053, 4.298933029175, 4.269775772095, 4.460028839111, 4.42603187561, 4.033329772949, 4.24411239624, 4.350168991089, 4.429989242554, 4.320943069458, 4.338175964355, 4.235773849487, 4.291209793091, 4.695648193359, 4.469025421143, 4.603707504272, 4.722597885132, 4.703507995605, 4.544751739502, 4.685567092896, 4.536740493774, 4.8740650177, 4.868990707397, 4.790534973145, 4.955137634277, 4.811549758911, 4.648141479492, 4.636721038818, 4.8352394104, 4.835529708862, 4.594055938721, 4.7346824646, 4.817284011841, 4.999242401123, 4.700812530518, 4.754711151123, 5.172610092163, 5.235391235352, 4.95241394043, 4.821824645996, 5.234642410278, 4.926895523071, 4.925301361084, 4.820500946045, 4.917132949829, 4.871397018433, 4.991929244995, 5.198739624023, 5.218975830078, 5.02686920166, 5.055498504639, 5.201330947876, 5.08977394104, 5.905376434326, 5.281586837769, 5.352125930786, 5.577616119385, 5.216376876831, 5.154947662354, 5.572798538208, 5.163168334961, 5.390545272827, 5.470174026489, 5.464821624756, 5.310091018677, 5.291172409058, 5.358726119995, 5.620496368408, 5.194181442261, 5.256386566162, 5.42172203064, 5.440140914917, 5.548281097412, 5.626251220703, 5.356834793091, 5.253130722046, 5.318742752075, 5.68006477356, 5.68572845459, 5.467984771729, 5.327723312378, 5.590407562256, 5.636714172363, 5.679320144653, 5.682856750488, 5.750899887085, 5.857872772217, 5.596611404419, 5.49202041626, 6.071594619751, 5.688243103027, 5.833169555664, 5.672495651245, 5.636151123047, 5.875593566895, 5.751121902466, 5.79578742981, 5.879794692993, 5.921696472168, 5.862273788452, 5.651864624023, 6.193706512451, 5.848851394653, 6.049449920654, 5.691621780396, 5.655501937866, 6.177133178711, 5.928916168213, 5.861355209351, 5.92149848938, 6.118215179443, 6.032846832275, 6.379351425171, 6.078337860107, 6.159463882446, 6.060427856445, 6.513017272949, 6.440522003174, 5.501903915405, 6.59582824707, 5.84377822876, 5.90313949585, 6.261034011841, 6.062595748901, 5.723009109497, 5.456116867065, 5.958324432373, 6.129544448853, 6.073327636719, 5.971546554565, 5.942286300659, 6.143724060059, 6.0670337677, 6.08553352356, 6.097022628784, 6.097808074951, 6.144799804688, 5.540007019043, 6.283581542969, 6.281435394287, 6.163754272461, 6.42406463623, 5.901816558838, 6.588552093506, 5.949268341064, 6.222561264038, 6.06954460144, 6.449098968506, 6.054425048828, 6.072605895996, 6.145413970947, 6.119645309448, 5.799216842651, 5.911767196655, 6.079314041138, 6.620789337158, 6.17272605896, 6.12167930603, 6.346855926514, 6.178206634521, 5.939904403687, 6.161736679077, 6.044065475464, 5.958396911621, 6.377970504761, 6.233358764648, 5.947356033325, 6.151121139526, 5.891180038452, 6.925098419189, 6.242875671387, 5.939756011963, 6.08385848999, 6.36522026062, 5.2107421875, 5.767859268188, 5.996054077148, 5.481103134155, 5.897212982178, 5.919049835205, 6.089249420166, 5.81148109436, 5.834842681885, 5.61812286377, 6.000196075439, 6.0643699646, 6.102388000488, 5.942763519287, 6.273902893066, 5.833169174194, 5.915376281738, 6.045357513428, 5.673691558838, 5.801525497437, 5.735981750488, 6.015894699097, 5.976083374023, 6.489854431152, 5.849991989136, 5.93392791748, 6.017129516602, 5.981259918213, 6.247099304199, 6.223910903931, 5.954475402832, 6.421701812744, 5.81136932373, 5.84252204895, 5.899547195435, 5.714159393311, 5.808564376831, 5.860989379883, 5.590309143066, 5.655537033081, 5.617249679565, 5.840449905396, 5.597567749023, 5.722720718384, 5.760615158081, 5.864346313477, 5.744559860229, 5.691886901855, 6.041994094849, 5.902842712402, 5.98380279541, 5.801394271851, 5.767325973511, 6.188642501831, 5.942957305908, 5.972537994385, 6.032473754883, 5.719231796265, 5.826833343506, 6.664742279053, 6.155096817017, 5.799701690674, 6.241035842896, 6.164431381226, 6.013113021851, 5.968277740479, 6.298058319092, 6.077303695679, 5.887189865112, 5.662783432007, 5.852509307861, 5.79832611084, 6.186330413818, 6.031255340576, 5.715073776245, 5.722459411621, 6.036476516724, 6.002643203735, 6.182195281982, 5.662850952148, 5.868334579468, 5.724560165405, 5.975210952759, 6.01456451416, 5.882150650024, 5.794535827637, 5.970975875854, 5.613739776611, 5.788278579712, 6.099000167847, 5.868279266357, 5.775360107422, 5.888514328003, 6.062854766846, 5.865188980103, 6.091522979736, 5.904286193848, 5.961211013794, 6.333650588989, 5.793334579468, 5.989757156372, 6.268152999878, 5.82173500061, 5.86480140686, 5.658504486084, 6.345817184448, 5.946770095825, 6.06580657959, 5.944828033447, 5.882643127441, 5.771848678589, 6.236356735229, 6.013126754761, 6.501286315918, 5.599529647827, 5.997425460815, 5.931896972656, 5.851745605469, 5.822393417358, 6.127079772949, 6.004335403442, 5.661632537842, 5.960472106934, 5.837683486938, 6.022089385986, 5.661967468262, 5.909827804565, 5.844833374023, 5.719762039185, 6.12232170105, 6.645365142822, 5.998783493042, 5.851502609253, 6.093084716797, 6.308781814575, 5.848572540283, 5.948519897461, 5.804048919678, 6.091474151611, 5.633238601685, 5.801024627686, 5.418580245972, 5.809906387329, 5.558165359497, 5.429398345947, 6.05997428894, 5.946970367432, 6.047135925293, 5.961177825928, 5.90788230896, 6.033740615845, 5.857928466797, 5.822309494019, 5.939750671387, 6.354515838623, 5.781274032593, 6.037635040283, 5.570983505249, 5.754800033569, 5.826644134521, 6.023974227905, 5.630793380737, 6.143116760254, 6.444304656982, 6.247612762451, 6.012269210815, 6.335196685791, 5.908707046509, 6.151155471802, 5.958019638062, 6.044861602783, 6.408984375, 6.125494384766, 6.195886230469, 5.727857589722, 6.364492797852, 5.77258605957, 5.816670608521, 6.307266616821, 5.708534240723, 5.708294296265, 6.23169593811, 5.776567459106, 5.916439819336, 5.811206054688, 6.028968048096, 5.92444229126, 6.067213821411, 5.923828125, 5.798350906372, 5.965497970581, 5.929954910278, 5.957558059692, 5.885297393799, 5.891138458252, 6.238974761963, 6.252387237549, 6.842907714844, 5.812997055054, 6.009622955322, 5.749491882324, 5.794826889038, 6.05415687561, 6.012434768677, 6.120937347412, 5.545516967773, 6.398160934448, 5.376750564575, 6.253303909302, 5.754130935669, 5.659018325806, 6.153592681885, 5.84563369751, 5.911034011841, 5.809372329712, 6.005610656738, 6.019454574585, 5.796855163574, 5.754432678223, 5.861554336548, 5.966553497314, 5.886725997925, 6.003915786743, 6.089763641357, 5.960628509521, 5.950080490112, 5.966487121582, 5.915520477295, 6.06058883667, 5.659120178223, 6.151445770264, 6.485125732422, 6.072692489624, 6.016611862183, 6.191917419434, 5.927819442749, 5.780850982666, 5.451056289673, 5.807326507568, 5.990872955322, 5.665190887451, 5.566494369507, 6.287158584595, 6.176008605957, 6.284346389771, 5.96229095459, 6.130126953125, 5.424837875366, 5.852186965942, 5.823039627075, 5.994487762451, 6.232426452637, 5.861870956421, 5.804808425903, 6.331642913818, 6.126299285889, 5.798169708252, 6.034689331055, 6.085572433472, 5.486646652222, 5.867000198364, 5.799021530151, 5.914880752563, 5.958319473267, 6.251190567017, 5.933433914185, 6.074985122681, 5.962911987305, 5.894404983521, 6.249425888062, 6.448860168457, 6.132403182983, 6.075061035156, 6.588989257813, 6.442337799072, 6.095115661621, 6.034513473511, 5.787791061401, 6.187595367432, 5.94766998291, 5.831318283081, 6.095761871338, 6.067388916016, 5.837226486206, 5.779539108276, 5.935031509399, 5.959790420532, 5.79460067749, 6.164888381958, 6.59461517334, 5.582851791382, 6.078456115723, 5.641095352173, 6.152025604248, 6.190531921387, 6.187731170654, 5.643822479248, 6.025988388062, 5.852779006958, 6.025779724121, 5.628345870972, 5.96216506958, 6.109199905396, 5.681100082397, 5.66587677002, 5.233234786987, 5.99764289856, 5.721488571167, 5.471377563477, 6.018132400513, 6.091384506226, 5.950129699707, 5.983947372437, 5.593114852905, 5.817736816406, 5.634126281738, 5.851600646973, 5.826190567017, 5.898962020874, 6.034480285645, 6.06058807373, 5.86462097168, 5.739625549316, 5.962073135376, 5.597003173828, 5.818897628784, 5.76922454834, 6.099938201904, 5.892625808716, 5.804953384399, 5.796352386475, 5.839940261841, 5.673241043091, 5.937405014038, 5.710807418823, 6.0633934021, 5.893880462646, 5.643441390991, 5.86628036499, 6.494966888428, 5.855453109741, 5.786143112183, 5.991150283813, 6.028709793091, 6.029784011841, 5.757773590088, 5.909748458862, 5.921598815918, 5.700205993652, 6.214100646973, 5.734306716919, 5.820137023926, 5.755303192139, 5.961964416504, 5.921611404419, 5.927919006348, 5.741962432861, 5.917492675781, 6.224605178833, 5.885103988647, 5.998156738281, 5.677878189087, 5.888108825684, 5.855653762817, 5.594520568848, 6.174001312256, 5.928536224365, 5.859134292603, 5.784489059448, 5.888423156738, 5.822407150269, 6.094902038574, 6.044956588745, 6.026028442383, 6.035558700562, 6.023594665527, 6.237533187866, 5.527682495117, 5.766287231445, 5.894077682495, 6.045457839966, 6.157605361938, 5.889031600952, 5.628659820557, 6.121823501587, 6.02911491394, 5.893016815186, 5.831595611572, 5.572897720337, 5.679089736938, 5.772768783569, 5.778559875488, 6.30316734314, 6.065715026855, 5.990181350708, 5.805543136597, 6.053877639771, 5.785356903076, 5.652982330322, 5.896006011963, 5.818085861206, 5.926160049438, 5.966408920288, 6.069553756714, 6.103318405151, 5.894269180298, 6.103009796143, 5.7629322052, 5.751735305786, 6.028192520142, 5.759773635864, 5.901726531982, 6.175210952759, 5.953536987305, 5.624029159546, 6.023745346069, 5.907445907593, 5.668124008179, 6.178409194946, 5.638032913208, 5.966818237305, 5.796720504761, 5.811803436279, 5.893891906738, 5.985636138916, 5.853226852417, 5.721022415161, 5.879979324341, 5.659490966797, 5.99174156189, 5.762585830688, 5.65078086853, 5.684661865234, 6.053663635254, 5.604690933228, 5.300579071045, 5.911630630493, 6.184924316406, 6.019853210449, 5.822630310059, 5.82579536438, 5.667847824097, 5.890697479248, 6.362613296509, 6.20951499939, 6.032376861572, 6.191938781738, 5.904029846191, 5.630874633789, 6.138665771484, 6.003563690186, 5.518809890747, 5.914489746094, 6.211354827881, 6.278536605835, 5.719791030884, 5.718468093872, 5.777182388306, 5.893362045288, 5.821670913696, 5.736783599854, 5.840157318115, 5.78373260498, 5.930310058594, 5.939417648315, 5.943627166748, 5.777351760864, 6.236197280884, 5.955088043213, 5.823483276367, 5.869470596313, 5.855351257324, 5.894317245483, 5.86802444458, 5.767866516113, 5.911873245239, 5.72299041748, 5.923050308228, 5.855834197998, 5.965231704712, 5.930427932739, 6.049476623535, 6.019758224487, 5.827627563477, 6.407372283936, 5.82479133606, 6.256640625, 5.908429718018, 6.067202377319, 5.448181152344, 5.859871673584, 5.945162963867, 5.666421508789, 6.101559066772, 5.897914123535, 5.855170822144, 6.173234558105, 6.282143783569, 5.870171356201, 5.764140319824, 5.631594085693, 5.626194000244, 5.673588180542, 5.431533432007, 5.504228591919, 6.063069152832, 5.798753356934, 6.253336715698, 6.03602180481, 5.765099334717, 6.288468933105, 6.017116928101, 5.913372421265, 5.888938903809, 6.362012481689, 6.362012481689, 5.88592300415, 5.769106674194, 6.155171966553, 5.832852554321, 5.302421188354, 5.780003738403, 6.254262924194, 5.662872695923, 5.981010437012, 5.867696762085, 5.888048171997, 6.063705825806, 6.584851074219 };
        private int _testValIdx = 0;
        private Random random = new Random();
        #endregion

        #region ctor
        /// <summary>
        /// Creates a new <see cref="AquaI2C"/>
        /// </summary>
        public EmptyI2CController(double tankWidth, double tankHeight, double tankDepth)
            : base(tankWidth, tankHeight, tankDepth)
        {
        }
        #endregion

        #region Properties
        public I2CDevice I2CSensor { get; private set; }
        public float FinalCapMeasure1 { get; private set; }
        public float FinalCapMeasure2 { get; private set; }
        public float FinalCapMeasure3 { get; private set; }
        public OneRegisterRead DataIn { get; private set; }
        #endregion

        #region Methods
        
        /// <summary>
        /// Resets the I2C port
        /// </summary>
        public void Reset()
        {
            Console.WriteLine($"WTR----------------{DateTime.Now}: {nameof(Reset)}");
        }

        /// <summary>
        /// Gets the height of the water in the tank
        /// </summary>
        /// <returns>A <see cref="double"/> representing the height</returns>
        public override void GetWaterHeight(object state)
        {
            double avgVal = 0;
            if (_calabrate == true)
            {
                if (_testValIdx > _testVals.Length - 1)
                    _testValIdx = 0;

                double testVal = _testVals[_testValIdx];
                avgVal = Average(testVal);
                _testValIdx++;
            }
            //Console.WriteLine($"avgVal----------------{DateTime.Now}: {avgVal}");
            //Console.WriteLine($"WTR----------------{DateTime.Now}: {testVal}");
            Console.WriteLine($"{avgVal}");
            WaterHeight = avgVal;
        }

        /// <summary>
        /// Calabrates the water level sensor
        /// </summary>
        public void CalabrateSensor()
        {
            _calabrate = true;
            Console.WriteLine($"{nameof(CalabrateSensor)}");
        }
        #endregion
    }
}
