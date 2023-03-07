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
    // Change this url to the archive of a specific console
    // The VGMusic archive says to link directly to their main page, thus this url leads to this page.
    await page.goto('http://www.vgmusic.com/');
    await page.waitForLoadState('networkidle');

    const locator = await page.locator('a[href$=".mid"]');
    
    let fileName, download;
    let len = await locator.all();
    // NOTE: Please limit the amount of downloads, bandwidth is expensive
    for (let i = 0; i < len.length; i++) {
        fileName = await locator.nth(i).getAttribute("href");
        [ download ] = await Promise.all([
            page.waitForEvent('download'),
            locator.nth(i).click()
        ]);
        await download.saveAs(privateData.path+fileName);
        await download.path();
        await page.waitForTimeout(20);
    }

    await browser.close();
})();