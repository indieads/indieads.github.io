var AdResolutions =
{
    // Standard mobile banner size
    Banner_320x50 : {w : 320, h : 50},
    // These you will already have if you release on steam
    Landscape_231x87 : {w : 231, h : 87},
    Landscape_460x215 : {w : 460, h : 215},
    Landscape_616x253 : {w : 616, h : 353},
    Portrait_374x448 : {w : 374, h : 448},
    Portrait_600x900 : {w : 600, h : 900},
}

const indieads_Foldernames =
[
  "spacevoyage",
  "calciumchaos",
  "outlawkingdom",
  "corr"
]


var indieads_Count = 0;

function spawnIndieAd_ImageOnly(container, resolution)
{
    indieads_Count += 1;
    
    var imgElement = document.createElement("img");
    imgElement.className = "indiead-img"
    imgElement.id = `indieads-img-${indieads_Count}`;
    
    container.append(imgElement);
    
    fetchIndieAd("", imgElement, resolution);
    
}

function spawnIndieAd(container, resolution)
{
    indieads_Count += 1;
    
    var linkElement = document.createElement("a");
    linkElement.target = "_blank";
    linkElement.className = "indiead-link"
    linkElement.id = `indieads-link-${indieads_Count}`;
    
    var imgElement = document.createElement("img");
    imgElement.className = "indiead-img"
    imgElement.id = `indieads-img-${indieads_Count}`;
    
    linkElement.append(imgElement);
    container.append(linkElement);
    
    fetchIndieAd(linkElement.id, imgElement, resolution)
}

async function fetchIndieAd(link_elementID, img_element, resolution) {
    
    let foldername = indieads_Foldernames[Math.floor(Math.random() * indieads_Foldernames.length)];

    // before when we didnt pass the element, ID was the way to get the reference back
    //document.getElementById(img_elementID).src = `https://indieads.github.io/stnemesitrevda/${foldername}/${resolution.w}x${resolution.h}.png`;
    img_element.src = `https://indieads.github.io/stnemesitrevda/${foldername}/${resolution.w}x${resolution.h}.png`;
    
    // only continue if we were provided with a link element
    if (link_elementID == "")
        return;

    fetch(`https://indieads.github.io/stnemesitrevda/${foldername}/href`)
        .then((response) => {
            if (!response.ok)
            {
                return "Fetch Failed Error";
            }
            return response.text(); 
        })
        .then((response) => {
            document.getElementById(link_elementID).href = `${response}`
        })
}
