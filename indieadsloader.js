



function loadAdGallery()
{

    var indieads = document.body.getElementsByClassName("indieads");
    
    for (let i = 0; i < indieads.length; i++) {
        
        classnames = indieads[i].classList;
        
        if (classnames.contains("320x50"))
        {
            spawnIndieAd(indieads[i], AdResolutions.Banner_320x50);
            continue;
        }
        if (classnames.contains("231x87"))
        {
            spawnIndieAd(indieads[i], AdResolutions.Landscape_231x87);
            continue;
        }
        if (classnames.contains("460x215"))
        {
            spawnIndieAd(indieads[i], AdResolutions.Landscape_460x215);
            continue;
        }
        if (classnames.contains("616x253"))
        {
            spawnIndieAd(indieads[i], AdResolutions.Landscape_616x253);
            continue;
        }
        if (classnames.contains("374x448"))
        {
            spawnIndieAd(indieads[i], AdResolutions.Portrait_374x448);
            continue;
        }
        if (classnames.contains("600x900"))
        {
            spawnIndieAd(indieads[i], AdResolutions.Portrait_600x900);
            continue;
        }
        
        //console.log(ad)
    }
}


loadAdGallery();

// Will open new page on click
spawnIndieAd(ad_1, AdResolutions.Banner_320x50);

// Not clickable
spawnIndieAd_ImageOnly(ad_2, AdResolutions.Banner_231x87);






