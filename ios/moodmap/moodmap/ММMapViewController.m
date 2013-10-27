//
//  ММMapViewController.m
//  moodmap
//
//  Created by Roman Putsykovich on 10/27/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import "ММMapViewController.h"

@interface __MapViewController ()

@end

@implementation __MapViewController

- (void)viewDidLoad {
    [super viewDidLoad];
	// Do any additional setup after loading the view.
    
    [self.webView loadRequest:[NSURLRequest requestWithURL:[NSURL fileURLWithPath:[[NSBundle mainBundle] pathForResource:@"map" ofType:@"html"]isDirectory:NO]]];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
