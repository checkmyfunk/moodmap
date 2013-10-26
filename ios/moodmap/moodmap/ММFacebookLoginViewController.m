//
//  ММFacebookLoginViewController.m
//  moodmap
//
//  Created by Roman Putsykovich on 10/26/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import "ММFacebookLoginViewController.h"
#import "ММRootTabBarController.h"

@interface __FacebookLoginViewController () <FBLoginViewDelegate>

@property (weak, nonatomic) IBOutlet FBLoginView *loginView;

@end

@implementation __FacebookLoginViewController

static NSString * const FacebookUserName = @"FBUserName";
static NSString * const FacebookProfileID = @"FBProfileID";
static NSString * const EmailKey = @"email";
static NSString * const PublishActionsKey = @"publish_actions";


- (void)viewDidLoad {
    [super viewDidLoad];
	// Do any additional setup after loading the view.
    self.loginView.publishPermissions = @[PublishActionsKey, EmailKey];
    self.loginView.defaultAudience = FBSessionDefaultAudienceFriends;
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    return interfaceOrientation == UIInterfaceOrientationPortrait;
}

- (void)transitionToMainViewController {
    
     __RootTabBarController *viewController = (__RootTabBarController*) [self.storyboard instantiateViewControllerWithIdentifier:@"rootTabBarConntroller"];
    [self presentViewController:viewController animated:YES completion:nil];
}

#pragma mark - FBLoginViewDelegate

- (void)loginViewShowingLoggedInUser:(FBLoginView *)loginView {
    // first get the buttons set for login mode
    [self transitionToMainViewController];
}

- (void)loginViewFetchedUserInfo:(FBLoginView *)login user:(id<FBGraphUser>)user {
    
    // here we use helper properties of FBGraphUser to dot-through to first_name and
    // id properties of the json response from the server; alternatively we could use
    // NSDictionary methods such as objectForKey to get values from the my json object
    
    id username = [NSString stringWithFormat:@"%@, %@", user.first_name, user.last_name];
    [[NSUserDefaults standardUserDefaults] setObject:username forKey:FacebookUserName];
    [[NSUserDefaults standardUserDefaults] setObject:user.id forKey:FacebookProfileID];
}

- (void)loginViewShowingLoggedOutUser:(FBLoginView *)loginView {

}

- (void)loginView:(FBLoginView *)loginView handleError:(NSError *)error {
    // see https://developers.facebook.com/docs/reference/api/errors/ for general guidance on error handling for Facebook API
    // our policy here is to let the login view handle errors, but to log the results
   // NSLog(@"FBLoginView encountered an error=%@", error);
}

@end
