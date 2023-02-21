const playwright = require('playwright');
// If you wanna use this file, include a private.json
// with a path attribute containing the whole path to your save location.
const privateData = require("./private.json");
(async () => {
    const browser = await playwright.chromium.launch({
        headless: false
    });
    
    const context = await browser.newContext({ acceptDownloads: true });
    const page = await context.newPage();
    await page.goto('http://vgmusic.com/music/console/nintendo/nes/');
    await page.waitForLoadState('networkidle');

    const locator = await page.locator('a[href$=".mid"]');
    
    let fileName, download;
    for (let i = 0; i < 50; i++) {
        fileName = await locator.nth(i).getAttribute("href");
        [ download ] = await Promise.all([
            page.waitForEvent('download'),
            locator.nth(i).click()
        ]);
        await download.saveAs(privateData.path+fileName);
        await download.path();
    }

    await browser.close();
})();